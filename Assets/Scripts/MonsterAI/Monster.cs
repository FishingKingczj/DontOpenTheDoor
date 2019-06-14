using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Monster : MonoBehaviour
{
    public Animator anim;

    [Header("player PressureSystem variable")]
    public bool visiable = false;
    public float timer_Standard = 1.0f;
    public float pressurePointIncrement = 1.0f;
    public string isBattle = "notBattle";

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
    public float monster_beforeAttack = 1.0f;

    /************************/

    private State state = new State();
    public string AIPath = "Text/BaseAI";
    private MonsterAI monsterAI;
    //private MonsterParameter monsterParameter;

    private float timer = 0;
    private Vector2 birthposiotion = new Vector2();
    private GameObject aimObject;
    private Vector2 aimPoint;
    private ArrayList way = new ArrayList();
    private int way_step = 0;

    protected void PerceptualLayer()
    {
        
    }
    protected void DecisionLayer()
    {
        Debug.Log(state.issueType);
        Debug.Log(state.stateType);
        Debug.Log(timer);
        if (state.issueType.Equals(MonsterState.canAttack) && !state.stateType.Equals(MonsterState.beforeAttack) && !state.stateType.Equals(MonsterState.attack) && !state.stateType.Equals(MonsterState.skill)) timer = -1.0f;
        if (state.issueType.Equals(MonsterState.seePlayer) && (state.stateType.Equals(MonsterState.hangOut)|| state.stateType.Equals(MonsterState.goBack))) {
            timer = -1.0f;
        }

        if (state.stateType.Equals(MonsterState.hangOut)){
            if (Vector2.Distance(aimPoint, (Vector2)transform.position) < 0.6f)
                timer = -1.0f;
        }
        if (state.stateType.Equals(state.intentionType.Equals(MonsterState.goBack)))
        {
            if (Vector2.Distance(aimPoint, (Vector2)transform.position) < 0.6f)
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
            else if (state.intentionType.Equals(MonsterState.beforeAttack))
            {
                timer = monster_beforeAttack;
            }
            else if (state.intentionType.Equals(MonsterState.hangOut))
            {
                timer = monster_hangOutTime;
                aimPoint = new Vector2(transform.position.x + 2*(Random.value-0.5f) * monster_hangOutScale, transform.position.y + 2 * (Random.value - 0.5f) * monster_hangOutScale);
            }
            else if (state.intentionType.Equals(MonsterState.goBack))
            {
                timer = monster_goBackTime;
                aimPoint = birthposiotion;
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
            anim.SetInteger("state", 0);
            autoMove((Vector2)aimObject.transform.position, monster_walkSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.rush))
        {
            anim.SetInteger("state", 1);
            straightMove(aimPoint, monster_rushSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.beforeAttack))
        {
            anim.SetInteger("state", 2);
        }
        else if (state.intentionType.Equals(MonsterState.attack))
        {
            attack();
        }
        else if (state.intentionType.Equals(MonsterState.skill))
        {
            skill();
        }
        else if (state.intentionType.Equals(MonsterState.goBack))
        {
            anim.SetInteger("state", 0);
            if (Vector2.Distance(aimPoint, (Vector2)transform.position) < 0.5f) timer = -1.0f;
            straightMove(aimPoint, monster_goBackSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.hangOut))
        {
            anim.SetInteger("state", 0);
            straightMove(aimPoint, monster_hangOutSpeed);
        }
        else if (state.intentionType.Equals(MonsterState.stay))
        {
            anim.SetInteger("state", 0);
        }
        else {
            //Debug.Log("error action");
        }
    }

    

    private void attack() {
        Debug.Log("怪物触碰玩家->怪物进入攻击模式并通知玩家进入逃生模式");
        isBattle = "battle";
        this.GetComponent<Monster_Battle>().StartAttack(GameObject.Find("player").gameObject);
    }

    private void skill()
    {
        if (GameObject.Find("RoomLoader").GetComponent<RoomLoader>().playerRoom.name.Equals(transform.parent.name))
            PlayerEvent.die();
    }

    private void faceTo(Vector2 targetPoint) {
        float size;
        if (transform.localScale.x > 0)
            size = transform.localScale.x;
        else
            size = -transform.localScale.x;
        if (targetPoint.x < transform.position.x)
        {
            
                transform.localScale = new Vector3(-size, transform.localScale.y, transform.localScale.z);
        }
        else {
            if (transform.localScale.y > 0)
                transform.localScale = new Vector3(size, transform.localScale.y, transform.localScale.z);
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
        birthposiotion = transform.localPosition + transform.parent.position;

        monsterAI = new MonsterAI(AIPath);

    }

    // Update is called once per frame
    void Update()
    {
        birthposiotion = transform.localPosition + transform.parent.position;

        Debug.Log(state.stateType);
        Debug.Log(state.issueType);
        if (isBattle == "battle") {
            timer = 5.0f;
            return;
        }

        if (isBattle == "afterBattle") {
            timer -= Time.deltaTime;
            if (timer < 0) {
                isBattle = "notBattle";
                state.issueType = "seeNothing";
                state.stateType = "hangOut";
            }
                
            return;
        }

        

        if (visiable) Timer_AddPressurePoint();

        DecisionLayer();
        AciotnLayer();
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject feeledObject = collider.gameObject;
        if (feeledObject.tag.Equals("Player")) {
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
        //Debug.Log(state.issueType);
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        GameObject feeledObject = collider.gameObject;
        if (feeledObject.tag.Equals("Player"))
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
        //Debug.Log(state.issueType);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        GameObject feeledObject = collider.gameObject;
        if (feeledObject.tag.Equals("Player"))
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
        //Debug.Log(state.issueType);
    }

    private void OnBecameVisible()
    {
        visiable = true;
    }
    private void OnBecameInvisible()
    {
        visiable = false;
        timer_Standard = 1.0f;
    }

    private void Timer_AddPressurePoint() {
            if (timer_Standard <= 0)
            {
                GameObject.Find("player").GetComponent<Player_BuffManager>().AddPressurePoint(pressurePointIncrement);
                timer_Standard = 1.0f;
            }
            else
                timer_Standard -= Time.deltaTime;
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

public class MonsterAI
{
    public Dictionary<string, string> ai = new Dictionary<string, string>();
    public MonsterAI() {
        
    }
    public MonsterAI(string AIPath)
    {
        string[] lines = ReadInText.readin(AIPath);
        int count = 0;
        while (count < lines.Length-1)
        {
            ai.Add(lines[count + 0], lines[count + 1]);
            count = count + 2;
        }

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
    public static string beforeAttack = "beforeAttack";
    public static string attack = "attack";
    public static string skill = "skill";

    public static string seeNothing = "seeNothing";
    public static string seePlayer = "seePlayer";
    public static string seeDoor = "seeDoor";
    public static string seeCollision = "seeCollision";
    public static string canAttack = "canAttack";

    public static string error = "error";
}
