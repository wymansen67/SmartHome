using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace SmartHome.Models
{
    public class Room
    {
       public Tv tv { get; set; }
        public Sound sound { get; set; }
        public Conditioner conditioner { get; set; }
        public Light light { get; set; }
    }
}

public class Tv
{
    public int Channel { get; set; }
    public int Volume { get; set; }
    public bool On { get; set; }

    public string OnString => On switch
    {
        false => "Выключен",
        true => "Включен"
    };
}

public class Sound
{
    public int Volume { get; set; }
    public bool IsPlaying { get; set; }
    public string Current { get; set; }

    public string IsPlayingString => IsPlaying switch
    {
        false => "Выключен",
        true => "Играет"
    };
}

public class Conditioner
{
    public double Temperature { get; set; }
    public double RoomTemperature { get; set; }
    public bool On { get; set; }

    public string OnString => On switch
    {
        false => "Выключен",
        true => "Включен"
    };
}

public class Light
{
    public int Status { get; set; }
    public int Color { get; set; }

    [JsonIgnore]
    public string StatusString => Status switch
    {
        0 => "Выключено",
        1 => "Слабое освещение",
        2 => "Среднее освещение",
        3 => "Полное освещение",
        _ => "Неизвестно"
    };

    [JsonIgnore]
    public string ColorString => Color switch
    {
        0 => "Обычный",
        1 => "Горячий",
        2 => "Холодный",
        _ => "Неизвестно"
    };
}

