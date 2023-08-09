using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Net.WebRequestMethods;

[CreateAssetMenu]
public class GDPRConfig : GenericConfig<GDPRConfig>
{
    public string termsOfServiceURL = "https://pastebin.com/i0R2vf6b";
    public string privacyPolicyURL = "https://pastebin.com/D8ffb5Q7";
}
