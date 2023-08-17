using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class RelayController : MonoBehaviour
{
    public async Task SignIn()
    {
        try
        {
            await UnityServices.InitializeAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
            };
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        catch (AuthenticationException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async Task CreateRelay()
    {
        try
        {
            Allocation alloc = await RelayService.Instance.CreateAllocationAsync(7);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(alloc.AllocationId);

            Debug.Log("Join code: " + joinCode);

            RelayServerData relayData = new RelayServerData(alloc, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    [Command]
    public async void JoinRelay(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with code: " + joinCode);
            JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayData = new RelayServerData(alloc, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

            NetworkManager.Singleton.StartClient();

            Debug.Log("Joined successfully");
        }
        catch (RelayServiceException e)
        {
            // failed to join using joincode, joincode is wrong?

            // show error message

            // go back to offline mode
            PopupError popup = PopupsManager.Instance.CreatePopup(PopupError.POPUP_PATH).GetComponent<PopupError>();
            popup.Init("Failed to join host", "Back Home", () =>
            {
                GameNetworkManager.Instance.StartOffline();
            });

            Debug.Log(e);
        }
    }
}
