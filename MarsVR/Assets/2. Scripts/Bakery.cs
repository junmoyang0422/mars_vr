using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Bakery : MonoBehaviour
{
    private PlayerCtrl player;
    private GameObject oven;
    private GameObject bowl;
    private GameObject flour;
    private GameObject egg;
    private GameObject water;
    private GameObject buter;
    private GameObject yeast;
    private SceneCtrl sceneCtrl;
    private AnchorCtrl[] anchorList = new AnchorCtrl[2];
    public List<string> scriptList = new List<string>();

    [SerializeField]
    private int tutorialNum = 0;

    private GameObject nextBtn;

    public GameObject anchorPrefab;
    private Vector3 offset;
    private Vector3 ovenOffset;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerCtrl>();
        oven = GameObject.Find("Oven");
        sceneCtrl = GameObject.Find("SceneCtrl").GetComponent<SceneCtrl>();
        bowl = GameObject.Find("Bowl");
        flour = GameObject.Find("Flour");
        egg = GameObject.Find("Egg");
        water = GameObject.Find("Water");
        buter = GameObject.Find("Buter");
        yeast = GameObject.Find("Yeast");
        
        nextBtn = transform.Find("NextBtn").gameObject;

        scriptList = FileIO.ReadScript("Bakery");
        if(scriptList == null)
        {
            gameObject.GetComponentInChildren<Text>().text = Application.persistentDataPath + ", " + Application.dataPath;
        }
        offset = Vector3.up * 0.2f;
        ovenOffset = Vector3.up * 1.0f; 

        if (PlayerPrefs.GetInt("OncePlayed") == 1)
        {
            tutorialNum = 21;
        }
        else
        {
            GameManager.instance.InitialList();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (tutorialNum) {
            case 0:
                PlayerPrefs.SetInt("LevelReceipt", 1);
                break;
            
            case 3:
                nextBtn.SetActive(false);
                if (anchorList[0] == null)
                {
                    anchorList[0] = CreateAnchor(bowl.transform.position + offset);
                }
                // 컵을 집을때
                if (bowl.GetComponent<OVRGrabbable>().isGrabbed)
                {
                    nextBtn.SetActive(true);
                    anchorList[0].EndAnchor();
                    anchorList[0] = null;
                    tutorialNum++;
                }
                break;
            
            case 6:
                nextBtn.SetActive(false);
                if (anchorList[0] == null)
                {
                    anchorList[0] = CreateAnchor(flour.transform.position + offset);
                }
                if (flour.GetComponent<OVRGrabbable>().isGrabbed)
                {
                    nextBtn.SetActive(true);
                    anchorList[0].EndAnchor();
                    anchorList[0] = null;
                    tutorialNum++;
                }
                break;
           
            case 10:
                nextBtn.SetActive(false);
                if (CheckAmount(BottleType.FLOUR, 199.5f))
                {
                    nextBtn.SetActive(true);
                    tutorialNum++;
                }
                break;
            
       
            //물, 이스트 넣기
            case 12:
                nextBtn.SetActive(false);
                if (anchorList[0] == null)
                    anchorList[0] = CreateAnchor(water.transform.position + offset);
                if (anchorList[1] == null)
                    anchorList[1] = CreateAnchor(yeast.transform.position + offset);
                if (CheckAmount(BottleType.WATER, 99.5f) && CheckAmount(BottleType.YEAST, 2.5f)) {
                    for (int i = 0; i < 2; i++)
                    {
                        anchorList[i].EndAnchor();
                        anchorList[i] = null;
                    }
                    nextBtn.SetActive(true);
                    tutorialNum++;
                }
                break;
            
            
            case 16:
                nextBtn.SetActive(false);
                if (anchorList[0] == null)
                {
                    anchorList[0] = CreateAnchor(oven.transform.position + ovenOffset);
                }
                if (oven.GetComponent<EvaluateManager>().isEnd)
                {
                    nextBtn.SetActive(true);
                    anchorList[0].EndAnchor();
                    anchorList[0] = null;
                    tutorialNum++;
                }
                break;

            case 20:
                PlayerPrefs.SetInt("OncePlayed", 1);
                PlayerPrefs.SetInt("Level1Score", (int)Mathf.Round(player.score));
                sceneCtrl.ToBaker();
                break;
            
            case 24:
                nextBtn.SetActive(false);
                PlayerPrefs.SetInt("LevelReceipt", 2);
                if (anchorList[0] == null)
                {
                    anchorList[0] = CreateAnchor(oven.transform.position + ovenOffset);
                }
                if (oven.GetComponent<EvaluateManager>().isEnd)
                {
                    nextBtn.SetActive(true);
                    anchorList[0].EndAnchor();
                    anchorList[0] = null;
                    tutorialNum++;
                }
                break;
            case 25:
                PlayerPrefs.SetInt("Level2Score", (int)Mathf.Round(player.score));
                PlayerPrefs.SetString("CurScene", SceneManager.GetActiveScene().name);
                PlayerPrefs.SetInt("OncePlayed", 0);
                break;
            case 26:
                sceneCtrl.ToScore();
                break;
        }
        if (tutorialNum < scriptList.Count)
            gameObject.GetComponentInChildren<Text>().text = scriptList[tutorialNum];
    }

    [ContextMenu("Next Script")]
    public void NextScript()
    {
        tutorialNum++;
    }

    private AnchorCtrl CreateAnchor(Vector3 pos)
    {
        AnchorCtrl temp = Instantiate(anchorPrefab, pos, Quaternion.identity).GetComponent<AnchorCtrl>();
        temp.originPos = pos;
        return temp;
    }

  
    private bool CheckAmount(BottleType bottleType, float amount)
    {
        float result = 0;
        for (int i = 0; i < bowl.GetComponent<CupCtrl>().receipt.Count; i++)
        {
            if (bowl.GetComponent<CupCtrl>().receipt[i].Equals(bottleType))
            {
                result += bowl.GetComponent<CupCtrl>().amounts[i];

            }
        }
        return result > amount;
    }
}
