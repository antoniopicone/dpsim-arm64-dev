using System;

namespace DomainModel.Classes.Simulations;

public class Simulation {
    public Guid id { get; set;}
    public UdpEndpointSim endpoint_source { get; set; }
    public UdpEndpointSim endpoint_dest { get; set; }
    public string time_step { get; set; }
    public string time_period_excecution { get; set; }
    public int frequency_band { get; set; }
}