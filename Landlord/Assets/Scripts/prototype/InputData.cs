using UnityEngine;

public class InputData: IInput
{
    public bool IsFinish { get; internal set; }
    public InputType inputType;
    public object interactiveObject;

    public IInteractable Selected()
    {
        throw new System.NotImplementedException();
    }

    public void Cancel()
    {
        throw new System.NotImplementedException();
    }

    public void SetSelected(GameObject obj)
    {
        throw new System.NotImplementedException();
    }
}

public enum InputType
{
    MoveToTile, //移動
    Interact, //互動
    Magic, //施法

}