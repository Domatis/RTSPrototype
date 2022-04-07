using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{

    public static MouseCursorManager instance;

    public enum MouseCursors {Neutral,CollectWood,CollectMeal,CollectGold,Attack,Construction}
    //Data for each cursor.
    [SerializeField] private Texture2D neutralMouseCursor;
    [SerializeField] private Texture2D collectWoodCursor;
    [SerializeField] private Texture2D collectMealCursor;
    [SerializeField] private Texture2D collectGoldCursor;
    [SerializeField] private Texture2D attackCursor;
    [SerializeField] private Texture2D constructionCursor;

    private MouseCursors currentCursorType = MouseCursors.Neutral;

    
    public MouseCursors CurrentCursortype { get{return currentCursorType;}}

    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        SetCursor(currentCursorType);
    }


    public void SetCursor(MouseCursors cursor)
    {
        switch(cursor)
        {
            case MouseCursors.Neutral:
            currentCursorType = MouseCursors.Neutral;
            Cursor.SetCursor(neutralMouseCursor,Vector2.zero,CursorMode.Auto);
            break;

            case MouseCursors.CollectGold:
            currentCursorType = MouseCursors.CollectGold;
            Cursor.SetCursor(collectGoldCursor,Vector2.zero,CursorMode.Auto);
            break;

            case MouseCursors.CollectMeal:
            currentCursorType = MouseCursors.CollectMeal;
            Cursor.SetCursor(collectMealCursor,Vector2.zero,CursorMode.Auto);
            break;

            case MouseCursors.CollectWood:
            currentCursorType = MouseCursors.CollectWood;
            Cursor.SetCursor(collectWoodCursor,Vector2.zero,CursorMode.Auto);
            break;

            case MouseCursors.Attack:
            currentCursorType = MouseCursors.Attack;
            Cursor.SetCursor(attackCursor,Vector2.zero,CursorMode.Auto);
            break;

            case MouseCursors.Construction:
            currentCursorType = MouseCursors.Construction;
            Cursor.SetCursor(constructionCursor,Vector2.zero,CursorMode.Auto);
            break;
        }
    }
}
