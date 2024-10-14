using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class ChangeSceneButton : MonoBehaviour
    {
        public string nombreEscena; //Asignar en el inspector, debe ser exactamente el nombre de la escena, y la escena debe estar el los build settings
        public void ChangeScene()
        {
            SceneManager.LoadScene(nombreEscena);
        }
    }
}
