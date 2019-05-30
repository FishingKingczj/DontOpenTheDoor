using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Monster : MonoBehaviour
{
    //monster parameter
    public float monster_attackTime = 1.0f;
    public float monster_attackScale = 1.0f;
    public float monster_rushTime = 1.0f;
    public float monster_rushSpeed = 2.0f;
    public float monster_walkTime = 0.9f;
    public float monster_walkSpeed = 1.0f;
    public float monster_hangOutTime = 1.0f;
    public float monster_hangOutSpeed = 1.0f;
    public float monster_hangOutScale = 10.0f;
    private float monster_goBackTime = 1000000.0f;
    public float monster_goBackSpeed = 1.0f;

    /************************/

    private State state = new State();
    private MonsterAI monsterAI = new MonsterAI();
    //private MonsterParameter monsterParameter;

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
        if (state.issueType.Equals(MonsterState.seePlayer) && (state.stateType.Equals(MonsterState.hangOut)|| state.stateType.Equals(MonsterState.goBack))) {
            timer = -1.0f;
        }

        if (timer < 0)
        {
            state.intentionType = monsterAI.getIntentionType(state.stateType + "_" + state.issueType);
            state.stateType = state.intentionType;

            if (state.intentionType.Equals(MonsterState.walk))
            {
                aimObject = GameObject.Find("player");
                timer = monster_walkTime;
            }
            else if (state.intentionType.Equals(MonsterState.rush))
            {
                aimObject = GameObject.Find("player");
                aimPoint = (Vector2)aimObject.transform.position;
                timer = monster_walkTime;
            }
            else if (state.intentionType.Equals(MonsterState.attack))
            {
                timer = monster_attackTime;
            }
            else if (state.intentionType.Equals(MonsterState.hangOut))
            {
                timer = monster_hangOutTime;
                aimPoint = new Vector2(transform.position.x + 2*(Random.value-0.5f) * monster_hangOutScale, transform.position.y + 2 * (Random.value - 0.5f) * monster_hangOutScale);
            }
            else if (state.intentionType.Equals(MonsterState.goBack))
            {
                timer = monster_goBackTime;
                aimPoint = brithposiotion;
            }
            else
                timer = -1.0f;
            
        }

        timer = timer - Time.deltaTime;
    }

    protected void AciotnLayer()
    {
        if (state.intentionType.Equals(MonsterState.walk))
        {
            autoMove((Vector2)aimObject.transform.position, monster_walkSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.rush))
        {
            straightMove(aimPoint, monster_rushSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.attack))
        {
            Debug.Log("Monster is attacking!!!!");
            SceneManager.LoadScene(0);
        }
        else if (state.intentionType.Equals(MonsterState.goBack))
        {
            if (Vector2.Distance(aimPoint, (Vector2)transform.position) < 0.5f) timer = -1.0f;
            straightMove(aimPoint, monster_goBackSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.hangOut))
        {
            straightMove(aimPoint, monster_hangOutSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.stay))
        {

        }
        else {
            Debug.Log("error action");
        }
    }

    private void faceTo(Vector2 targetPoint) {
        if (targetPoint.x < transform.position.x)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        else {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
    }

    private void straightMove(Vector2 targetPoint,float speed) {
        faceTo(targetPoint);
        Vector2 direction = (targetPoint - (Vector2)transform.position).normalized;
        float x = transform.position.x + direction.x * speed * Time.deltaTime;
        float y = transform.position.y + direction.y * speed * Time.deltaTime;
        transform.position = new Vector3(x, y, transform.position.z);
        return;
    }

    private void autoMove(Vector2 targetPoint,float speed) {
        faceTo(targetPoint);
        if (Vector2.Distance((Vector2)transform.position, targetPoint) < monster_attackScale)
                return;
        Vector2[] ways = GetComponent<AStar>().AStarFindWay((Vector2)transform.position, targetPoint);
        if (ways.Length < 2) return;
        straightMove(ways[1],speed);
        return;
    }

    // Start is called before the first frame update
    void Start()
    {
        brithposiotion = transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        DecisionLayer();
        AciotnLayer();
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
                if (Vector2.Distance((Vector2) feeledObject.transform.position, (Vector2) transform.position)< monster_attackScale)
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
                if (Vector2.Distance((Vector2)feeledObject.transform.position, (Vector2)transform.position) < monster_attackScale)
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

/*public class MonsterParameter
{
    private Dictionary<string, float> para = new Dictionary<string, float>();
    public MonsterParameter(float _attackTime, float _attackScale,float _walkTime,float _walkSpeed,float _rushTime,float _rushSpeed)
    {
        //读入配置表
        para.Add("attack", _attackTime);
        para.Add("attackScale", _attackScale);
        para.Add("walk", _walkTime);
        para.Add("walkSpeed", _walkSpeed);
        para.Add("rush", _rushTime);
        para.Add("rushSpeed", _rushSpeed);
    }
    public float getParameter(string parameterName)
    {
        if (para.ContainsKey(parameterName))
        {
            return para[parameterName];
        }
        return -1.0f;
    }
}*/

public class MonsterAI
{
    private Dictionary<string, string> ai = new Dictionary<string, string>();
    public MonsterAI()
    {
        //读入配置表
        ai.Add("stay_seeNothing", "hangOut");
        ai.Add("stay_seeDoor", "hangOut");
        ai.Add("stay_seeCollision", "hangOut");
        ai.Add("stay_seePlayer","walk");
        ai.Add("stay_canAttack", "attack");

        ai.Add("walk_seeNothing", "hangOut");
        ai.Add("walk_seeDoor", "hangOut");
        ai.Add("walk_seeCollision", "walk");
        ai.Add("walk_seePlayer", "rush");
        ai.Add("walk_canAttack", "attack");

        ai.Add("rush_seeNothing", "hangOut");
        ai.Add("rush_seeDoor", "hangOut");
        ai.Add("rush_seeCollision", "walk");
        ai.Add("rush_seePlayer", "walk");
        ai.Add("rush_canAttack", "attack");

        ai.Add("attack_seeNothing", "hangOut");
        ai.Add("attack_seeDoor", "hangOut");
        ai.Add("attack_seeCollision", "walk");
        ai.Add("attack_seePlayer", "walk");
        ai.Add("attack_canAttack", "attack");

        ai.Add("hangOut_seeNothing", "goBack");
        ai.Add("hangOut_seeDoor", "goBack");
        ai.Add("hangOut_seeCollision", "walk");
        ai.Add("hangOut_seePlayer", "rush");
        ai.Add("hangOut_canAttack", "attack");

        ai.Add("goBack_seeNothing", "hangOut");
        ai.Add("goBack_seeDoor", "hangOut");
        ai.Add("goBack_seeCollision", "walk");
        ai.Add("goBack_seePlayer", "walk");
        ai.Add("goBack_canAttack", "attack");

        ai.Add("error_seeNothing", "error");
        ai.Add("error_seeDoor", "error");
        ai.Add("error_seeCollision", "error");
        ai.Add("error_seePlayer", "error");
        ai.Add("error_canAttack", "error");
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
    public static string attack = "attack";

    public static string seeNothing = "seeNothing";
    public static string seePlayer = "seePlayer";
    public static string seeDoor = "seeDoor";
    public static string seeCollision = "seeCollision";
    public static string canAttack = "canAttack";

    public static string error = "error";
}
