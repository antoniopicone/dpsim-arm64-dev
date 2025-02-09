import socket
import json
import cmath
import math
import threading
import time
import os

# Configurazione circuito
R = 10.0
L = 0.1
f = 50.0
omega = 2 * math.pi * f
SAMPLE_FREQUENCY = 1000  # 1 kHz
PERIOD = 1.0 / SAMPLE_FREQUENCY

# Configurazione
HOST_DEST = os.getenv('HOST_DEST', 'villas_lab_a')
HOST_SOURCE = os.getenv('HOST_SOURCE', '0.0.0.0')
PORT_DEST = int(os.getenv('PORT_DEST', '12001'))
PORT_SOURCE = int(os.getenv('PORT_SOURCE', '12000'))
SAMPLE_FREQUENCY = int(os.getenv('SAMPLE_FREQUENCY', '1000'))
PERIOD = 1.0 / SAMPLE_FREQUENCY

class VoltageHandler:
    def __init__(self):
        self.lock = threading.Lock()
        self.voltage = {'real': 0.0, 'imag': 0.0}
        self.new_data = False

    def update(self, new_voltage):
        with self.lock:
            self.voltage = new_voltage
            self.new_data = True

    def get(self):
        with self.lock:
            return self.voltage.copy()

voltage_handler = VoltageHandler()
sequence_counter = 0

def calculate_current(voltage_phasor):
    V = complex(voltage_phasor['real'], voltage_phasor['imag'])
    Z = R + 1j * omega * L
    I = V / Z
    return {'real': I.real, 'imag': I.imag}

def udp_receiver():
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((HOST_SOURCE, PORT_SOURCE))
    
    while True:
        try:
            data, _ = sock.recvfrom(1024)
            json_data = json.loads(data.decode())
            
            if isinstance(json_data, list):
                for entry in json_data:
                    if 'data' in entry and isinstance(entry['data'], list) and len(entry['data']) > 0:
                        voltage_data = entry['data'][0]
                        if 'real' in voltage_data and 'imag' in voltage_data:
                            voltage_handler.update({
                                'real': float(voltage_data['real']),
                                'imag': float(voltage_data['imag'])
                            })
            print(f"Received from {HOST_DEST}: {json_data}")
            
        except (json.JSONDecodeError, KeyError, ValueError) as e:
            print(f"Errore nel parsing JSON: {str(e)}")
        except Exception as e:
            print(f"Errore receiver: {str(e)}")

def simulation_loop():
    global sequence_counter
    sock_tx = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    next_time = time.time()
    
    while True:
        try:
            # Timing preciso
            next_time += PERIOD
            sleep_time = next_time - time.time()
            if sleep_time > 0:
                time.sleep(sleep_time)
            
            # Calcolo corrente
            current_voltage = voltage_handler.get()
            current = calculate_current(current_voltage)
            
            # Costruzione payload JSON
            payload = [{
                "sequence": sequence_counter,
                "data": [{
                    "real": round(current['real'], 4),
                    "imag": round(current['imag'], 4)
                }]
            }]
            
            sock_tx.sendto(json.dumps(payload).encode(), (HOST_DEST, PORT_DEST))
            print(f"Sent to {HOST_DEST}: {payload}")
            
            sequence_counter += 1
            
        except Exception as e:
            print(f"Errore simulazione: {str(e)}")

def setup_realtime_scheduling():
    param = os.sched_param(os.sched_get_priority_max(os.SCHED_RR))
    os.sched_setscheduler(0, os.SCHED_RR, param)
    print(f"Scheduling configurato: {os.sched_getscheduler(0)}")

if __name__ == "__main__":
    setup_realtime_scheduling()
    
    receiver_thread = threading.Thread(target=udp_receiver, daemon=True)
    receiver_thread.start()
    
    simulation_loop()