using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;
using DataManager;
using System.Runtime.Serialization.Formatters.Binary;

namespace VGP142.PlayerInputs
{

    public class GameStateManager : MonoBehaviour
    {
        private Player player;
        public Text hintText;

        public Save save;
        private string path;

        private void Start()
        {
            player = FindObjectOfType<Player>();
            hintText.gameObject.SetActive(false);
        }

        public void SaveGame()
        {

            SaveByPlayerPrefs();
            //SaveBySerialization();
            //SaveByJSON();
            SampleSave();
        }

        public void LoadGame()
        {

            LoadByPlayerPrefs();
            //LoadByDeSerialization();
            //LoadByJSON();
            //LoadByXML();
        }

        private void SampleSave()
        {
            Save save = createSaveGameObject();
            XmlDocument xmlDocument = new XmlDocument();

            #region CreateXML elements

            //MARKER XmlElement : one of the most common nodes
            XmlElement root = xmlDocument.CreateElement("Save");
            root.SetAttribute("FileName", "File_01");//OPTIONAL

            XmlElement playerPosXElement = xmlDocument.CreateElement("PlayerPositionX");
            playerPosXElement.InnerText = save.playerPositionX.ToString();
            root.AppendChild(playerPosXElement);

            XmlElement playerPosYElement = xmlDocument.CreateElement("PlayerPositionY");
            playerPosYElement.InnerText = save.playerPositionY.ToString();
            root.AppendChild(playerPosYElement);

            XmlElement playerPosZElement = xmlDocument.CreateElement("PlayerPositionZ");
            playerPosZElement.InnerText = save.playerPositionZ.ToString();
            root.AppendChild(playerPosZElement);

            #endregion

            xmlDocument.AppendChild(root);//Add the root and its children elements to the XML Document
            xmlDocument.Save(Application.dataPath + "/DataXML.text");

            path = (Application.dataPath + "/DataXML.text");
            GameData.SaveData(save, path);

            //if (File.Exists(path))
            //{
            //    Debug.Log("XML FILE SAVED");
            //}
        }

        private void LoadByXML()
        {
            if (File.Exists(Application.dataPath + "/DataXML.text"))
            {
                //LOAD THE GAME
                Save save = new Save();

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(Application.dataPath + "/DataXML.text");

                //path = (Application.dataPath + "/DataXML.text");
                //save = GameData.LoadData(save, path) as Save;

                XmlNodeList playerPosX = xmlDocument.GetElementsByTagName("PlayerPositionX");//<PlayerPositionX>...</PlayerPositionX>
                float playerPosXNum = float.Parse(playerPosX[0].InnerText);
                save.playerPositionX = playerPosXNum;

                XmlNodeList playerPosY = xmlDocument.GetElementsByTagName("PlayerPositionY");
                float playerPosYNum = float.Parse(playerPosY[0].InnerText);
                save.playerPositionY = playerPosYNum;

                XmlNodeList playerPosZ = xmlDocument.GetElementsByTagName("PlayerPositionZ");
                float playerPosZNum = float.Parse(playerPosZ[0].InnerText);
                save.playerPositionZ = playerPosZNum;

                player.transform.position = new Vector3(save.playerPositionX, save.playerPositionY, save.playerPositionZ);

                Debug.Log("Load XML");
            }
        }

        //MARKER SAVE AND LOAD BY SERIALIZATION
        private Save createSaveGameObject()
        {
            Save save = new Save();

            save.playerPositionX = player.transform.position.x;
            save.playerPositionY = player.transform.position.y;
            save.playerPositionZ = player.transform.position.z;
            return save;
        }

        //MARKER Object(Save Type) --> JSON(String)
        private void SaveByJSON()
        {
            Save save = createSaveGameObject();

            //CORE Returns (String Type) Object's data in JSON format
            string JsonString = JsonUtility.ToJson(save);//Convert SAVE Object into JSON(String Type)

            //CORE Write the "JsonString" to the "JSONData.text" file
            StreamWriter sw = new StreamWriter(Application.dataPath + "/JSONData.text");
            sw.Write(JsonString);//CORE Write a string to a stream

            sw.Close();

            Debug.Log("-=-=-=-SAVED-=-=-=-");
        }

        //MARKER JSON(STring) --> Object(Save Type)
        private void LoadByJSON()
        {
            if (File.Exists(Application.dataPath + "/JSONData.text"))
            {
                //LOAD THE GAME
                StreamReader sr = new StreamReader(Application.dataPath + "/JSONData.text");

                //CORE Read the text directly from the text file
                string JsonString = sr.ReadToEnd();

                sr.Close();

                //Convert JSON to the Object(save)
                Save save = JsonUtility.FromJson<Save>(JsonString);//Into the Save Object

                player.transform.position = new Vector3(save.playerPositionX, save.playerPositionY, save.playerPositionZ);

                Debug.Log("Load JSON");
            }
            else
            {
                Debug.Log("NOT FOUND FILE");
            }
        }



        //MARKER SAVE --> Stream of Bytes
        private void SaveBySerialization()
        {
            Save save = createSaveGameObject();//MARKER Create a SAVE instance will all the data for current status

            BinaryFormatter bf = new BinaryFormatter();

            //FileStream fileStream = File.Create(Application.persistentDataPath + "/Data.text");
            FileStream fileStream = File.Create(Application.dataPath + "/Data.text");

            bf.Serialize(fileStream, save);//Object to bytes

            fileStream.Close();
        }

        private void LoadByDeSerialization()
        {
            if (File.Exists(Application.persistentDataPath + "/Data.text"))
            {
                //LOAD THE GAME
                BinaryFormatter bf = new BinaryFormatter();

                //FileStream fileStream = File.Open(Application.persistentDataPath + "/Data.text", FileMode.Open);
                FileStream fileStream = File.Open(Application.dataPath + "/Data.text", FileMode.Open);

                Save save = bf.Deserialize(fileStream) as Save;//You have loaded your previous "save" object
                fileStream.Close();

                player.transform.position = new Vector3(save.playerPositionX, save.playerPositionY, save.playerPositionZ);
            }
            else
            {
                //REPORT THE ERROR
                Debug.Log("NOT FOUND THIS FILE");
            }
        }

        //MARKER SAVE AND LOAD BY PLAYERPREFS
        private void SaveByPlayerPrefs()
        {

            //SAVE the player gameobject position
            PlayerPrefs.SetFloat("PlayerPosX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerPosY", player.transform.position.y);
            PlayerPrefs.SetFloat("PlayerPosZ", player.transform.position.z);
            Debug.Log("SAVE THE DATA");
            StartCoroutine(DisplayHintCo("SAVED"));
        }

        private void LoadByPlayerPrefs()
        {
            if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
            {
                player.playerPosX = PlayerPrefs.GetFloat("PlayerPosX");
                player.playerPosY = PlayerPrefs.GetFloat("PlayerPosY");
                player.playerPosZ = PlayerPrefs.GetFloat("PlayerPosZ");
                StartCoroutine(DisplayHintCo("LOADED"));
                Debug.Log("LOAD THE DATA");
            }
            else
            {
                StartCoroutine(DisplayHintCo("NOT FOUND"));
            }

            player.transform.position = new Vector3(player.playerPosX, player.playerPosY, player.playerPosZ);
        }

        //MARKER THIS FUNCTION WILL BE TRIGGERED WHEN YOU PRESS THE SAVE OR LOAD BUTTON
        IEnumerator DisplayHintCo(string _message)
        {
            hintText.gameObject.SetActive(true);
            hintText.text = _message;
            yield return new WaitForSeconds(2);
            hintText.gameObject.SetActive(false);
        }
    }
}

