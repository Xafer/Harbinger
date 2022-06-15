using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    public static GameData Instance;

    public Player PlayerCharacter;
    public PlayerRobot PlayerRobotCharacter;

    [SerializeField] private List<CameraTremble> _trembleCameras = new List<CameraTremble>();

    [SerializeField] private PlayerControl PC;

    private void Awake()
    {
        Instance = this;
    }

    public void SetControlledCharacter(bool isPlayer)
    {
        PC.ControlledEntity = isPlayer ? (Entity)PlayerCharacter : (Entity)PlayerRobotCharacter;
    }

    public void Tremble(TrembleSource source)
    {
        foreach (CameraTremble ct in _trembleCameras)
            if(ct.isActiveAndEnabled)ct.Tremble(source);
    }
}
