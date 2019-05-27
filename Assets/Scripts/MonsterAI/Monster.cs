using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private State state = new State();
    private MonsterAI monsterAI = new MonsterAI();
    private MonsterParameter monsterParameter = new MonsterParameter();

    private float timer = 0;
    private Vector2 brithposiotion = new Vector2();
    private GameObject aimObject;
    private Vector2 aimPoint;
    private ArrayList way = new ArrayList();
    private int way_step = 0;

    protected void PerceptualLayer()
    {
        
    }
    protected void DecisionLayer()
    {
        if (state.issueType.Equals(MonsterState.canAttack)) timer = -1.0f;

        if (timer < 0)
        {
            state.intentionType = monsterAI.getIntentionType(state.stateType + "_" + state.issueType);
            state.stateType = state.intentionType;

            if (state.intentionType.Equals(MonsterState.walk))
            {
                aimObject = GameObject.Find("player");
            }
            else if (state.intentionType.Equals(MonsterState.rush)) {
                aimObject = GameObject.Find("player");
                aimPoint = (Vector2) aimObject.transform.position;
            }

            timer = monsterParameter.getParameter(state.intentionType);
        }

        timer = timer - Time.deltaTime;
    }

    protected void AciotnLayer()
    {
        if (state.intentionType.Equals(MonsterState.walk))
        {
            autoMove((Vector2)aimObject.transform.position);
        }
        else if (state.intentionType.Equals(MonsterState.rush))
        {
            simpleMove(aimPoint);
            if (Vector2.Distance((Vector2) transform.position, aimPoint)<0.1f) {
                timer = -1.0f;
            }
        }
        else if (state.intentionType.Equals(MonsterState.attack))
        {
            Debug.Log("Monster is attacking!!!!");
        }
        else if (state.intentionType.Equals(MonsterState.goBack))
        {
        }
        else if (state.intentionType.Equals(MonsterState.hangOut))
        {

        }
        else if (state.intentionType.Equals(MonsterState.stay))
        {

        }
    }

    private void simpleMove(Vector2 targetPoint) {
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        float x = transform.position.x + direction.x * monsterParameter.getParameter("speed") * Time.deltaTime;
        float y = transform.position.y + direction.y * monsterParameter.getParameter("speed") * Time.deltaTime;
        transform.position = new Vector3(x, y, transform.position.z);
        return;
    }

    private void autoMove(Vector2 targetPoint) {
        if (Vector2.Distance((Vector2)transform.position, targetPoint) < monsterParameter.getParameter("attackScale"))
                return;
        Vector2[] ways = GetComponent<AStar>().AStarFindWay((Vector2)transform.position, targetPoint);
        Debug.Log(11111111111);
        Debug.Log((Vector2)transform.position);
        Debug.Log(targetPoint);
        Debug.Log(ways[0]);
       

        if (ways.Length < 2) return;
        simpleMove(ways[1]);
        Debug.Log(ways[1]);

        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        brithposiotion = transform.position;

        aimObject = GameObject.Find("player");
        autoMove((Vector2)aimObject.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        /*DecisionLayer();
        AciotnLayer();*/
        autoMove((Vector2)aimObject.transform.position);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject feeledObject = collider.gameObject;
        if (feeledObject.name.Equals("player")) {
            RaycastHit2D[] objects = Physics2D.LinecastAll((Vector2) transform.position, (Vector2)feeledObject.transform.position);
            state.issueType = MonsterState.seePlayer;
            foreach (var obj in objects) {
                if (obj.collider.tag.Equals("Item_Door")) 
                    state.issueType = MonsterState.seeDoor;
                else if (obj.collider.tag.Equals("Item_Collision")) 
                    state.issueType = MonsterState.seeCollision;
            }
            if (state.issueType == MonsterState.seePlayer) {
                if (Vector2.Distance((Vector2) feeledObject.transform.position, (Vector2) transform.position)<monsterParameter.getParameter("attackScale"))
                    state.issueType = MonsterState.canAttack;
            }
        }
        Debug.Log(state.issueType);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        GameObject feeledObject = collider.gameObject;
        if (feeledObject.name.Equals("player"))
        {
            RaycastHit2D[] objects = Physics2D.LinecastAll((Vector2)transform.position, (Vector2)feeledObject.transform.position);
            state.issueType = MonsterState.seePlayer;
            foreach (var obj in objects)
            {
                if (obj.collider.tag.Equals("Item_Door"))
                    state.issueType = MonsterState.seeDoor;
                else if (obj.collider.tag.Equals("Item_Collision"))
                    state.issueType = MonsterState.seeCollision;
            }
            if (state.issueType == MonsterState.seePlayer)
            {
                if (Vector2.Distance((Vector2)feeledObject.transform.position, (Vector2)transform.position) < monsterParameter.getParameter("attackScale"))
                    state.issueType = MonsterState.canAttack;
            }
        }
        Debug.Log(state.issueType);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        GameObject feeledObject = collider.gameObject;
        if (feeledObject.name.Equals("player"))
        {
            RaycastHit2D[] objects = Physics2D.LinecastAll((Vector2)transform.position, (Vector2)feeledObject.transform.position);
            foreach (var obj in objects)
            {
                if (obj.collider.tag.Equals("Item_Door"))
                    state.issueType = MonsterState.seeDoor;
                else
                    state.issueType = MonsterState.seeNothing;
            }
        }
        Debug.Log(state.issueType);
    }
}

public class State
{

    public string stateType;
    public string issueType;
    public string intentionType;
    public State()
    {
        this.stateType = MonsterState.stay;
        this.issueType = MonsterState.seeNothing;
        this.intentionType = MonsterState.stay;
    }
    public State(string stateType, string issueType)
    {
        this.stateType = stateType;
        this.issueType = issueType;
    }
}

public class MonsterParameter
{
    private Dictionary<string, float> para = new Dictionary<string, float>();
    public MonsterParameter()
    {
        //读入配置表
        para.Add("speed", 10.0f);
        para.Add("attackScale", 1.5f);
        para.Add("attacking", 1.0f);
        para.Add("walk", 0.9f);
        para.Add("rush", 2.0f);
    }
    public float getParameter(string parameterName)
    {
        if (para.ContainsKey(parameterName))
        {
            return para[parameterName];
        }
        return -1.0f;
    }
}

public class MonsterAI
{
    private Dictionary<string, string> ai = new Dictionary<string, string>();
    public MonsterAI()
    {
        //读入配置表
        ai.Add("stay_seeNothing", "stay");
        ai.Add("stay_seeDoor", "stay");
        ai.Add("stay_seeCollision", "stay");
        ai.Add("stay_seePlayer","walk");
        ai.Add("walk_seePlayer", "rush");
        ai.Add("rush_seePlayer", "walk");
        ai.Add("rush_canAttack", "attacking");
        ai.Add("walk_canAttack", "attacking");
        ai.Add("attacking_seeNothing", "stay");
        ai.Add("attacking_seeDoor", "walk");
        ai.Add("attacking_seePlayer", "walk");
        ai.Add("attacking_seeCollision", "walk");
        ai.Add("attacking_canAttack", "attacking");
        ai.Add("error_seeDoor", "stay");
        ai.Add("error_seeCollision", "stay");
        ai.Add("error_seePlayer", "stay");
    }
    public string getIntentionType(string state_issue)
    {
        if (ai.ContainsKey(state_issue))
        {
            return ai[state_issue];
        }
        if (!state_issue.Contains("error"))
            Debug.Log(state_issue);
        return MonsterState.error;
    }
}

public static class MonsterState
{
    public static string stay = "stay";
    public static string hangOut = "hangOut";
    public static string walk = "walk";
    public static string rush = "rush";
    public static string goBack = "goBack";
    public static string attack = "attacking";

    public static string seeNothing = "seeNothing";
    public static string seePlayer = "seePlayer";
    public static string seeDoor = "seeDoor";
    public static string seeCollision = "seeCollision";
    public static string canAttack = "canAttack";

    public static string error = "error";
}
