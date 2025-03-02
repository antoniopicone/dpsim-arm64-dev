
import socket
import json
import math
import os
import dpsimpy
import time as time_module
import requests as api_request
import sys

# Configurazione
HOST_DEST = os.getenv('HOST_DEST', 'villas_lab_a')
HOST_SOURCE = os.getenv('HOST_SOURCE', '0.0.0.0')
PORT_DEST = int(os.getenv('PORT_DEST', '12001'))
PORT_SOURCE = int(os.getenv('PORT_SOURCE', '12000'))
TIME_STEP_MILLIS = float(os.getenv('TIME_STEP_MILLIS', '1'))
TAU_MILLIS = float(os.getenv('TAU_MILLIS', '1'))
V_REF_VS = float(os.getenv('V_REF_VS', '10000'))
FREQUENZA = float(os.getenv('FREQUENZA', '50'))
TIME_STOP = float(os.getenv('TIME_STOP', '1'))
ITERATIONS = int(float(os.getenv('TIME_STOP', '1'))*1000/(TIME_STEP_MILLIS))

def get_simulation_data():
    
    parameters = { "id": "b9f1d4ea-e3d4-4924-9e44-59eff4fc64b6"}
    
    response = api_request.get(url="http://api_orchestrator:8080/api/Simmulation", params=parameters)
    response.raise_for_status()
    data = response.json()

    FREQUENZA = data["simulations"][0]["frequency_band"]
    TIME_STEP_MILLIS = int(data["simulations"][0]["time_step"])
    TAU_MILLIS = int(data["simulations"][0]["time_period_excecution"])
    HOST_DEST = data["simulations"][0]["endpoint_dest"]["host"]
    HOST_SOURCE = data["simulations"][0]["endpoint_source"]["host"]
    PORT_DEST = int(data["simulations"][0]["endpoint_dest"]["port"])
    PORT_SOURCE = int(data["simulations"][0]["endpoint_source"]["port"])

    return FREQUENZA, TIME_STEP_MILLIS, TAU_MILLIS, HOST_DEST, HOST_SOURCE, PORT_DEST, PORT_SOURCE

def start_simulation():

    name = 'VILLAS_test'

    # Nodes
    gnd = dpsimpy.dp.SimNode.gnd
    n1 =  dpsimpy.dp.SimNode('n1')
    n2 =  dpsimpy.dp.SimNode('n2')
    n3 =  dpsimpy.dp.SimNode('n3')
    
    # initialize node voltages as in simulunk
    n2.set_initial_voltage(complex(0,0))
    n3.set_initial_voltage(complex(0,0))

    # Components
    vs = dpsimpy.dp.ph1.VoltageSource('vs')
    vs.set_parameters(V_ref=complex(V_REF_VS,0)* math.sqrt(2))    
    
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
    print(f'LAB A TIMESTEP = {_time_step} ms')
    sim.set_time_step(_time_step)
    _time_stop = TIME_STOP
    sim.set_final_time(_time_stop)
    sim.start()
    
    return sim, l1, vload

def next_simulation(sim,l1,vload,voltage_phasor,sequence,time_step):

    inizio = time_module.perf_counter()

    #print(f"Applying voltage node n3 {str(Vn3)}")
    vload.set_parameters(V_ref=voltage_phasor)

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
    print(f"Sent current to {HOST_DEST}: {payload}")

    fine = time_module.perf_counter()
    tempo_esecuzione = fine - inizio
    
    if tempo_esecuzione <= (time_step*1000):
        print(f"Risolto LAB A in: {str(tempo_esecuzione*1000)} msec")
        time_module.sleep((TAU_MILLIS - TIME_STEP_MILLIS)/1000)
    
def udp_receiver(sim,l1,vload):
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
    sock.bind((HOST_SOURCE, PORT_SOURCE))
    sequence=0
    _time_step = TIME_STEP_MILLIS/1000
    while True:
        try:
            data, _ = sock.recvfrom(1024)
            vs = json.loads(data.decode())
            v_real = vs[0]['data'][0]['real']
            v_imag = vs[0]['data'][0]['imag']
            #sequence = vs[0]['sequence']
            sequence = sequence+1
            
            print(f"Received from {HOST_DEST}: {vs}")
            if (sequence <= ITERATIONS):
                next_simulation(sim,l1,vload,complex(v_real,v_imag),sequence,_time_step)
            else:
                sys.exit()

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
    #FREQUENZA, TIME_STEP_MILLIS, TAU_MILLIS, HOST_DEST, HOST_SOURCE, PORT_DEST, PORT_SOURCE = get_simulation_data()
    setup_realtime_scheduling()
    sim, l1, vload = start_simulation()
    udp_receiver(sim, l1, vload)