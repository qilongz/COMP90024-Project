using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography.X509Certificates;

namespace FilterHospitals.Hospital
{
    [DataContract]
    public class Geometry
    {
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "coordinates")] public List<double> Coordinates { get; set; }
    }

    [DataContract]
    public class HospitalDescription
    {
        [DataMember(Name = "ogc_fid")] public int OgcFid { get; set; }

        [DataMember(Name = "hospital n")] public string HospitalName { get; set; }
        [DataMember(Name = "phone numb")] public string PhoneNumber { get; set; }
        [DataMember(Name = "street add")] public string StringAddress { get; set; }

        [DataMember(Name = "suburb")] public string Suburb { get; set; }
        [DataMember(Name = "postcode")] public int Postcode { get; set; }
        [DataMember(Name = "state")] public string State { get; set; }
        [DataMember(Name = "website")] public string Website { get; set; }
        [DataMember(Name = "descriptio")] public string Description { get; set; }
        [DataMember(Name = "emergency")] public int Emergency { get; set; }
        [DataMember(Name = "sector")] public string Sector { get; set; }
        [DataMember(Name = "beds")] public string Beds { get; set; }
        [DataMember(Name = "latitude")] public double Latitude { get; set; }
        [DataMember(Name = "longitude")] public double Longitude { get; set; }
        [DataMember(Name = "bbox")] public List<double> Bbox { get; set; }



    }

    [DataContract]
    public class Feature
    {
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "id")] public string Id { get; set; }
        [DataMember(Name = "geometry")] public Geometry Geometry { get; set; }
        [DataMember(Name = "geometry_name")] public string GeometryName { get; set; }
        [DataMember(Name = "properties")] public HospitalDescription Description { get; set; }
    }

    [DataContract]
    public class Properties2
    {
        [DataMember(Name = "name")] public string Name { get; set; }
    }

    [DataContract]
    public class Crs
    {
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "properties")] public Properties2 Properties { get; set; }
    }


    [DataContract]
    public class EmergencyHospitals
    {
        [DataMember(Name = "type")] public string Type { get; set; }
        [DataMember(Name = "totalFeatures")] public int TotalFeatures { get; set; }
        [DataMember(Name = "features")] public List<Feature> Features { get; set; }
        [DataMember(Name = "crs")] public Crs Crs { get; set; }
        [DataMember(Name = "bbox")] public List<double> Bbox { get; set; }


        public static EmergencyHospitals Load(string gridFile)
        {
            using (var fs = File.Open(gridFile,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var ser = new DataContractJsonSerializer(typeof(EmergencyHospitals));
                return (EmergencyHospitals) ser.ReadObject(fs);
            }
        }
    }
}