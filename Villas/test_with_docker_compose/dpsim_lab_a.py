
import socket
import json
import math
import os
import dpsimpy
import time as time_module


# Configurazione
HOST_DEST = os.getenv('HOST_DEST', 'villas_lab_a')
HOST_SOURCE = os.getenv('HOST_SOURCE', '0.0.0.0')
PORT_DEST = int(os.getenv('PORT_DEST', '12001'))
PORT_SOURCE = int(os.getenv('PORT_SOURCE', '12000'))
TIME_STEP_MILLIS = int(os.getenv('TIME_STEP_MILLIS', '1'))
TAU_MILLIS = int(os.getenv('TAU_MILLIS', '1'))

def start_simulation(voltage_phasor,sequence):

    name = 'VILLAS_test'
    
    inizio = time_module.perf_counter()

    # Nodes
    gnd = dpsimpy.dp.SimNode.gnd
    n1 =  dpsimpy.dp.SimNode('n1')
    n2 =  dpsimpy.dp.SimNode('n2')
    
    # initialize node voltages as in simulunk
    n1.set_initial_voltage(complex(30, 0) * math.sqrt(2))
    n2.set_initial_voltage(complex(120, 0) * math.sqrt(2))
    
    # Components
    vs = dpsimpy.dp.ph1.VoltageSource('vs')
    vs.set_parameters(V_ref=voltage_phasor)
    print(f"Applying voltage {str(voltage_phasor)}")
    r1 = dpsimpy.dp.ph1.Resistor('r1')
    r1.set_parameters(R=1)
    l1 = dpsimpy.dp.ph1.Inductor('l1')
    l1.set_parameters(L=0.03)
    
    vs.connect([gnd, n1])
    r1.connect([n1, n2])
    l1.connect([n2, gnd])
    
    system = dpsimpy.SystemTopology(50, [gnd, n1, n2], [vs, r1, l1])
    
    
    sim = dpsimpy.Simulation(name)
    sim.set_domain(dpsimpy.Domain.DP)
    sim.set_system(system)
    _time_step = TIME_STEP_MILLIS/1000
    sim.set_time_step(_time_step)
    sim.set_final_time(_time_step) # eseguiamo una sola iterazione, quindi coincidente con il time step
    sim.start()
    sim.next()
    i_out = l1.attr("i_intf") 
    
    real_part = i_out.get()[0, 0].real  # Parte reale
    imag_part = i_out.get()[0, 0].imag  # Parte immaginaria
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
    print(f"Sent current to {HOST_DEST}: {payload}")

    fine = time_module.perf_counter()
    tempo_esecuzione = fine - inizio
    
    if tempo_esecuzione <= (_time_step*1000):
        print(f"Risolto LAB A in: {str(tempo_esecuzione*1000)} msec")
        time_module.sleep((TAU_MILLIS - TIME_STEP_MILLIS)/1000)
    
def udp_receiver():
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((HOST_SOURCE, PORT_SOURCE))
    sequence=0
    while True:
        try:
            sequence=sequence+1
            data, _ = sock.recvfrom(1024)
            vs = json.loads(data.decode())
            print(vs)
            v_real = vs[0]['data'][0]['real']
            v_imag = vs[0]['data'][0]['imag']
            print(f"Received from {HOST_DEST}: {vs}")
            start_simulation(complex(v_real,v_imag),sequence)
            
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