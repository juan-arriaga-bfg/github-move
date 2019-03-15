using Debug = IW.Logger;
using UnityEngine;

public class HighlightTaskCurrencySpend : ITaskHighlight
{
    public bool Highlight(TaskEntity task)
    {
        TaskCurrencySpendEntity currencyTask = task as TaskCurrencySpendEntity;
        if (currencyTask == null)
        {
            Debug.LogError("[TaskCurrencySpendEntity] => task is not TaskCurrencySpendEntity");
            return false;
        }

        if (string.IsNullOrEmpty(currencyTask.CurrencyName))
        {
            return false;
        }

        if (currencyTask.CurrencyName == Currency.Coins.Name)
        {
            return new HighlightTaskPointToEnergyPlusButton().Highlight(task);
        }

        if (currencyTask.CurrencyName == Currency.Crystals.Name)
        {
            return new HighlightTaskPointToShopButton().Highlight(task);
        }


        Debug.LogError($"[HighlightTaskCurrencySpend] => Unsupported Currency type: '{currencyTask.CurrencyName}'");

        return false;
    }
}