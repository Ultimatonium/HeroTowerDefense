using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class LobbyManager : NetworkLobbyManager {
    private MatchInfo gameMatchInfo = null;
    public Action OnPlayerJoined { get; set; }
    public static Action OnSceneReady { get; set; }

    private void Start() {
        EnableMatchMaker();
    }

    public void OnApplicationQuit() {
        ResetSelf();
    }

    private void EnableMatchMaker() {
        if (matchMaker == null) StartMatchMaker();
    }

    public void ResetSelf() {
        if (gameMatchInfo == null) return;
        if (matchMaker == null) return;
        matchMaker.DestroyMatch(gameMatchInfo.networkId, 0, OnDestroyMatch);
    }

    public override void OnDestroyMatch(bool success, string extendedInfo) {
        base.OnDestroyMatch(success, extendedInfo);
        if (success) {
            StopMatchMaker();
            StopHost();
        } else matchMaker.DropConnection(gameMatchInfo.networkId, gameMatchInfo.nodeId, 0, OnDropConnection);
    }

    public override void OnDropConnection(bool success, string extendedInfo) {
        StopMatchMaker();
    }

    public void GetFirstMatchListed() {
        EnableMatchMaker();
        ListMatches();
    }

    private void ListMatches() {
        matchMaker.ListMatches(0, 1, "", false, 0, 0, OnMatchList);
    }

    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList) {
        base.OnMatchList(success, extendedInfo, matchList);

        if (!success) {
            return;
        }

        if (matchList.Count > 0) {
            JoinMatch(matchList[0]);
            return;
        }

        CreateMatch();
    }

    private void JoinMatch(MatchInfoSnapshot match) {
        matchMaker.JoinMatch(match.networkId, "", "", "", 0, 0, OnMatchJoined);
    }

    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo) {
        base.OnMatchJoined(success, extendedInfo, matchInfo);

        if (!success) {
            return;
        }

        gameMatchInfo = matchInfo;
    }

    private void CreateMatch() {
        matchMaker.CreateMatch("Hero", 2, true, "", "", "", 0, 0, OnMatchCreate);
    }

    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo) {
        base.OnMatchCreate(success, extendedInfo, matchInfo);

        if (!success) {
            return;
        }

        gameMatchInfo = matchInfo;
    }

    public override void OnLobbyClientEnter() {
        base.OnLobbyClientEnter();
        UpdatePlayerInformation();
    }

    public override void OnLobbyServerConnect(NetworkConnection conn) {
        base.OnLobbyServerConnect(conn);
        UpdatePlayerInformation();
    }

    public override void OnServerDisconnect(NetworkConnection conn) {
        base.OnServerDisconnect(conn);
        UpdatePlayerInformation();
    }

    public override void OnClientDisconnect(NetworkConnection conn) {
        base.OnClientDisconnect(conn);
        ResetSelf();
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn) {
        base.OnLobbyClientDisconnect(conn);
        ResetSelf();
        UpdatePlayerInformation();
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn) {
        base.OnLobbyClientSceneChanged(conn);
        if (OnSceneReady != null) OnSceneReady();
    }

    public NetworkLobbyPlayer GetLocalNetworkLobbyPlayer() {
        foreach (var item in lobbySlots) {
            if (item.isLocalPlayer) return item;
        }
        return null;
    }

    private void UpdatePlayerInformation() {
        if (OnPlayerJoined == null) return;
        StartCoroutine(SetTimeout(.2f, () => {
            OnPlayerJoined();
        }));
    }

    private IEnumerator SetTimeout(float timeInSeconds, Action callback = null) {
        yield return new WaitForSeconds(timeInSeconds);
        if (callback != null) callback();
    }
}
