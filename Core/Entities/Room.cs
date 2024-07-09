using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities;
public class Room
{
    public int Id { get; set; }
    public string? RoomNumber { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }

}
