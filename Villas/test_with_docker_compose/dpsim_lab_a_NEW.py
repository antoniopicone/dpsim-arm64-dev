
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
V_REF_VS = float(os.getenv('V_REF_VS', '10000'))
FREQUENZA = float(os.getenv('FREQUENZA', '50'))

def start_simulation():

    name = 'VILLAS_test'

    # Nodes
    gnd = dpsimpy.dp.SimNode.gnd
    n1 =  dpsimpy.dp.SimNode('n1')
    n2 =  dpsimpy.dp.SimNode('n2')
    n3 =  dpsimpy.dp.SimNode('n3')
    
    # Components
    vs = dpsimpy.dp.ph1.VoltageSource('vs')
    vs.set_parameters(V_ref=complex(V_REF_VS,0))
    
    r1 = dpsimpy.dp.ph1.Resistor('r1')
    r1.set_parameters(R=1)
    
    l1 = dpsimpy.dp.ph1.Inductor('l1')
    l1.set_parameters(L=0.02)

    vload = dpsimpy.dp.ph1.VoltageSource('vload')
    
   
    vs.connect([gnd, n1])
    r1.connect([n1, n2])
    l1.connect([n2, n3])
    vload.connect([n3, gnd])
    
    system = dpsimpy.SystemTopology(FREQUENZA, [gnd, n1, n2, n3], [vs, r1, l1, vload])
    
    sim = dpsimpy.Simulation(name)
    sim.set_domain(dpsimpy.Domain.DP)
    sim.set_system(system)
    _time_step = TIME_STEP_MILLIS/1000
    sim.set_time_step(_time_step)
    sim.set_final_time(_time_step*1000) # eseguiamo una sola iterazione, quindi coincidente con il time step
    sim.start()
    
    return sim, l1, n3, vload

def next_simulation(sim,l1,n3,vload,voltage_phasor,sequence,time_step):

    inizio = time_module.perf_counter()

    #print(f"Applying voltage node n3 {str(Vn3)}")
    vload.set_parameters(V_ref=voltage_phasor)
    n3.set_initial_voltage(voltage_phasor)

    sim.next()
    sequence=sequence+1

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
    #print(f"Sent current to {HOST_DEST}: {payload}")

    fine = time_module.perf_counter()
    tempo_esecuzione = fine - inizio
    
    if tempo_esecuzione <= (time_step*1000):
        print(f"Risolto LAB A in: {str(tempo_esecuzione*1000)} msec")
        time_module.sleep((TAU_MILLIS - TIME_STEP_MILLIS)/1000)
    
def udp_receiver(sim,l1,n3,vload):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((HOST_SOURCE, PORT_SOURCE))
    sequence=0
    time_step = TIME_STEP_MILLIS/1000
    while True:
        try:
            data, _ = sock.recvfrom(1024)
            vs = json.loads(data.decode())
            #print(vs)
            v_real = vs[0]['data'][0]['real']
            v_imag = vs[0]['data'][0]['imag']
            sequence = vs[0]['sequence']
            
            #print(f"Received from {HOST_DEST}: {vs}")
            next_simulation(sim,l1,n3,vload,complex(v_real,v_imag),sequence,time_step)

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
    sim, l1, n3, vload = start_simulation()
    udp_receiver(sim, l1, n3, vload)