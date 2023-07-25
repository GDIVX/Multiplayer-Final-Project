using System.Collections;
using UnityEngine;

namespace Assets.Scripts.PlayerControls
{
    public interface IController 
    {
        public Vector2 GetMovementVector();

    }
}