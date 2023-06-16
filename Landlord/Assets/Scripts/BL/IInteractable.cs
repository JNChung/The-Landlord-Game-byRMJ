using System.Collections.Generic;

public interface IInteractable
{
    IEnumerable<string> GetChoice();
    IActionResult Be(string behavior);
    void InitializeChoice(IEnumerable<string> choice);

}
