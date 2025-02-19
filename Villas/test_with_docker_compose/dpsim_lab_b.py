import socket
import json
import math
import os
import time as time_module
import dpsimpy

# Configurazione
HOST_DEST = os.getenv('HOST_DEST', 'villas_lab_b')
HOST_SOURCE = os.getenv('HOST_SOURCE', '0.0.0.0')
PORT_DEST = int(os.getenv('PORT_DEST', '12002'))
PORT_SOURCE = int(os.getenv('PORT_SOURCE', '12003'))
TIME_STEP_MILLIS = int(os.getenv('TIME_STEP_MILLIS', '1'))
TAU_MILLIS = int(os.getenv('TAU_MILLIS', '1'))
# Tensione di bootstrap
BOOTSTRAP_VOLTAGE_REAL = float(os.getenv('BOOTSTRAP_VOLTAGE_REAL', '120.0'))
BOOTSTRAP_VOLTAGE_IMAG = float(os.getenv('BOOTSTRAP_VOLTAGE_IMAG', '0.0'))

def send_bootstrap_voltage(sequence):
    payload = [{
        "sequence": sequence,
        "data": [{
            "real": BOOTSTRAP_VOLTAGE_REAL,
            "imag": BOOTSTRAP_VOLTAGE_IMAG
        }]
    }]
    
    sock_tx = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock_tx.sendto(json.dumps(payload).encode(), (HOST_DEST, PORT_DEST))
    print(f"Sent bootstrap voltage to {HOST_DEST}: {payload}")

def start_simulation(current_phasor,sequence):
    
    name = 'VILLAS_test'
    
    inizio = time_module.perf_counter()

    # Nodes
    gnd = dpsimpy.dp.SimNode.gnd
    n1 = dpsimpy.dp.SimNode('n1')
    
    # Inizializzazione tensioni dei nodi
    n1.set_initial_voltage(complex(0, 0))
    
    # Components
    cs = dpsimpy.dp.ph1.CurrentSource('cs')
    cs.set_parameters(I_ref=current_phasor)
    print(f"Applying current {str(current_phasor)}")
    
    r1 = dpsimpy.dp.ph1.Resistor('r1')
    r1.set_parameters(R=1)
    
    # Connessione componenti
    cs.connect([gnd, n1])
    r1.connect([n1, gnd])
    
    # Setup sistema
    system = dpsimpy.SystemTopology(50, [gnd, n1], [cs, r1])
    
    # Setup simulazione
    sim = dpsimpy.Simulation(name)
    sim.set_domain(dpsimpy.Domain.DP)
    sim.set_system(system)
    _time_step = TIME_STEP_MILLIS/1000
    sim.set_time_step(_time_step)
    sim.set_final_time(_time_step)
    
    # Esecuzione simulazione
    sim.start()
    sim.next()
    
    # Lettura tensione ai capi del resistore
    v_out = n1.attr("v")
    
    real_part = v_out.real
    imag_part = v_out.imag
    
    payload = [{
        "sequence": sequence,
        "data": [{
            "real": real_part,
            "imag": imag_part
        }]
    }]
    
    # Invio risultato
    sock_tx = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock_tx.sendto(json.dumps(payload).encode(), (HOST_DEST, PORT_DEST))
    print(f"Sent voltage to {HOST_DEST}: {payload}")

    fine = time_module.perf_counter()
    tempo_esecuzione = fine - inizio
    
    if tempo_esecuzione <= (_time_step*1000):
        print(f"Risolto LAB B in: {str(tempo_esecuzione*1000)} msec")
        time_module.sleep((TAU_MILLIS - TIME_STEP_MILLIS)/1000)

def udp_receiver():
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((HOST_SOURCE, PORT_SOURCE))
    _time_step = TIME_STEP_MILLIS/1000
    sock.settimeout(TAU_MILLIS/1000)  # Timeout di TAU_MILLIS sec per il polling
    
    first_value_received = False
    sequence = 0
    while True:
        try:
            sequence=sequence+1
            
            if not first_value_received:
                # Invia tensione di bootstrap
                send_bootstrap_voltage(sequence)
                print("Waiting for first current value...")
            
            # Prova a ricevere dati
            data, _ = sock.recvfrom(1024)
            cs = json.loads(data.decode())
            i_real = cs[0]['data'][0]['real']
            i_imag = cs[0]['data'][0]['imag']
            print(f"Received from {HOST_DEST}: {cs}")
            
            # Imposta il flag dopo aver ricevuto il primo valore
            first_value_received = True

            # Esegui la simulazione con il valore ricevuto
            start_simulation(complex(i_real, i_imag),sequence)
            
        except socket.timeout:
            # Se non abbiamo ancora ricevuto il primo valore, continua il bootstrap
            if not first_value_received:
                continue
            else:
                # Se abbiamo già ricevuto almeno un valore, usa l'ultimo valore valido
                print("Timeout: no new current value received")
        
        except (json.JSONDecodeError, KeyError, ValueError) as e:
            print(f"Errore nel parsing JSON: {str(e)}")
        except Exception as e:
            print(f"Errore receiver: {str(e)}")

def setup_realtime_scheduling():
    param = os.sched_param(os.sched_get_priority_max(os.SCHED_RR))
    os.sched_setscheduler(0, os.SCHED_RR, param)
    print(f"Scheduling configurato: {os.sched_getscheduler(0)}")

if __name__ == "__main__":
    time_module.sleep(2)
    setup_realtime_scheduling()
    udp_receiver()
    
    