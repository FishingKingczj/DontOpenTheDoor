using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster_Battle : MonoBehaviour
{
    [Header("Attack Command")]
    public List<AttackGroup> attackGroup;
    private int attackGroupSize;

    [System.Serializable]
    public struct AttackGroup
    {
        public float attackTime;
        public float attackTimeRandomRange;

        public int[] attackSequenceIndex;
        public float[] probability;
    }

    [Header("Attak Sequence")]
    public List<AttackSequence> attackSequence;

    [System.Serializable]
    public struct AttackSequence
    {
        public int[] attackType;
    }

    // 默认下标 0->左攻击 1->右攻击 2->全屏攻击
    [Header("Attack Type")]
    public AttackType[] attackType = new AttackType[3];

    [System.Serializable]
    public struct AttackType
    {
        public float damage;
        public float energyExpendIfBlock;

        public float time_BefroeAttack;
        public float time_Attack;
        public float time_PerfectBlock;
    }

    [Header("Activating Attack Infomation")]
    public bool inAttack = false;

    public List<float> attackCommandTimeList;
    public List<int> attackCommandSequenceList;

    public float time_AttackCommand = 0;
    public float timer_AttackCommand = 0;

    [Header("Current AttackSequence Infomation")]
    public int currentAttackType = -1;

    public bool inAttackSequence = false;

    public bool inAttackingJudgment = false;
    public bool inPerfectBlockJudgement = false;

    public List<int> currentAttackSequence;
    public float time_CurrentAttackSequence = 0;
    public float timer_CurrentAttackSequence = 0;

    public List<float> time_CurrentAttackSequenceStart;
    public List<float> time_CurrentAttackSequenceStartAttack;
    public List<float> time_CurrentAttackSequencePerfectBlockEnd;
    public List<float> time_CurrentAttackSequenceEnd;

    [Header("Monster_BattleSceneObject")]
    public Monster_BattleScene monster_BattleScene;

    [Header("Player Infomation")]
    public GameObject player;
    public Joystick joystick;

    public bool finishedJudgement = false;
    public bool blocked = false;

    [Header("PressurePoint Configuration Variable")]
    public float pressurePointIncrementWhenHitted = 5.0f;

    public enum Judgement {
        None = -1,
        Roll = 0,
        Block,
        Hitted,
        HittedBlock,
        PerfectBlock
    };
    public Judgement currentPlayerJudgement = Judgement.None;

    void Start()
    {
        joystick = GameObject.Find("Canvas_UI").transform.Find("Joystick").gameObject.GetComponent<Joystick>();
        attackGroupSize = attackGroup.Count;

        monster_BattleScene = GameObject.Find("Canvas_UI").transform.Find("Canvas_Battle").transform.Find("Monster_BattleScene").gameObject.GetComponent<Monster_BattleScene>();
    }

    void FixedUpdate()
    {
        if (inAttack) {
            // 攻击指令已空 重新载入攻击指令 直到玩家逃脱
            if (attackCommandSequenceList.Count == 0 && !inAttackSequence)
            {
                LoadAttackInfomation();
            }
            else {
                // 到达攻击指令开始点 载入攻击序列
                if (attackCommandTimeList.Count != 0 && timer_AttackCommand > attackCommandTimeList[0] && !inAttackSequence)
                {
                    attackCommandTimeList.RemoveAt(0);
                    LoadAttackSequence();
                }
                else {
                    // 若当前正在执行攻击序列中 停止攻击指令时间线
                    if (!inAttackSequence)
                        timer_AttackCommand += Time.deltaTime;
                    // 此处执行攻击序列
                    else {
                        // 攻击序列执行完毕
                        if (timer_CurrentAttackSequence >= time_CurrentAttackSequence)
                        {
                            inAttackSequence = false;

                            inAttackingJudgment = false;
                            inPerfectBlockJudgement = false;
                        }
                        // 攻击序列执行中
                        else {
                            timer_CurrentAttackSequence += Time.deltaTime;

                            //检测玩家QTE操作
                            CheckPlayerOperation(currentAttackType);

                            // 到达 攻击结束点(一段攻击的最终点)
                            if (time_CurrentAttackSequenceEnd.Count != 0 && timer_CurrentAttackSequence >= time_CurrentAttackSequenceEnd[0])
                            {
                                // 关闭怪物战斗画面
                                monster_BattleScene.SetEnable(false);

                                Debug.Log("攻击结束");

                                time_CurrentAttackSequenceEnd.RemoveAt(0);
                                inAttackingJudgment = false;

                                // 执行检测判定结果
                                if (!finishedJudgement)
                                {
                                    ExecutePlayerJudgement(currentAttackType);
                                }

                                // 重设判定变量
                                finishedJudgement = false;
                                blocked = false;
                                currentPlayerJudgement = Judgement.None;
                            }

                            //到达 起始点(同时也为攻击判定的结束点)
                            if (time_CurrentAttackSequenceStart.Count != 0 && timer_CurrentAttackSequence >= time_CurrentAttackSequenceStart[0]) {
                                Debug.Log("攻击前摇开始 攻击类型 " + currentAttackSequence[0]);

                                currentAttackType = currentAttackSequence[0];
                                time_CurrentAttackSequenceStart.RemoveAt(0);

                                // 战斗画面怪物播放动画
                                monster_BattleScene.SetEnable(true);
                                monster_BattleScene.PlayAnimation(currentAttackType);

                                currentAttackSequence.RemoveAt(0);
                            }

                            //到达 攻击点(同时也为完美判定起始点)
                            if (time_CurrentAttackSequenceStartAttack.Count != 0 && timer_CurrentAttackSequence >= time_CurrentAttackSequenceStartAttack[0]) {
                                Debug.Log("攻击开始 并 开始完美判定");

                                inAttackingJudgment = true;
                                inPerfectBlockJudgement = true;

                                time_CurrentAttackSequenceStartAttack.RemoveAt(0);
                            }

                            //到达 完美判定结束点
                            if (time_CurrentAttackSequencePerfectBlockEnd.Count != 0 && timer_CurrentAttackSequence >= time_CurrentAttackSequencePerfectBlockEnd[0]) {
                                Debug.Log("完美判定结束");

                                inPerfectBlockJudgement = false;

                                time_CurrentAttackSequencePerfectBlockEnd.RemoveAt(0);

                                // 执行检测判定优先级结果
                                if (currentPlayerJudgement >= Judgement.Hitted && !finishedJudgement) {
                                    Debug.Log("完美时间段内 得到 高优先级 判断");
                                    ExecutePlayerJudgement(currentAttackType);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // 开始攻击(载入攻击信息)
    public void StartAttack(GameObject _player) {
        Debug.Log("怪兽开始攻击");
        player = _player;
        inAttack = true;

        player.SendMessage("EnterEscapeMode", this.gameObject, SendMessageOptions.DontRequireReceiver);
        LoadAttackInfomation();
    }

    // 结束攻击(玩家死亡或逃脱)
    public void EndAttack() {
        this.transform.GetComponent<Monster>().isBattle = false;
        Debug.Log("怪兽结束攻击");
        inAttack = false;
        player = null;

        attackCommandTimeList.Clear();
        attackCommandSequenceList.Clear();
        currentAttackSequence.Clear();
        timer_AttackCommand = 0;
        timer_CurrentAttackSequence = 0;
        time_AttackCommand = 0;
        timer_CurrentAttackSequence = 0;

        currentAttackType = -1;

        time_CurrentAttackSequenceStart.Clear();
        time_CurrentAttackSequenceStartAttack.Clear();
        time_CurrentAttackSequencePerfectBlockEnd.Clear();
        time_CurrentAttackSequenceEnd.Clear();

        inAttackSequence = false;
        inAttackingJudgment = false;
        inAttackSequence = false;
        blocked = false;
        finishedJudgement = false;

        currentPlayerJudgement = Judgement.None;

        monster_BattleScene.SetEnable(false);
    }

    // 载入攻击指令
    private void LoadAttackInfomation() {
        currentAttackType = -1;

        attackCommandTimeList.Clear();
        attackCommandSequenceList.Clear();

        time_AttackCommand = 0;
        timer_AttackCommand = 0;

        inPerfectBlockJudgement = false;
        inAttackingJudgment = false;

        for (int i = 0; i < attackGroupSize; i++)
        {
            // 载入攻击指令执行时间
            attackCommandTimeList.Add(Random.Range(attackGroup[i].attackTime - attackGroup[i].attackTimeRandomRange, attackGroup[i].attackTime + attackGroup[i].attackTimeRandomRange));
            time_AttackCommand = Mathf.Max(attackCommandTimeList[i]);

            // 载入攻击序列
            float range_Max = 0;
            for (int j = 0; j < attackGroup[i].probability.Length; j++)
            {
                range_Max += attackGroup[i].probability[j];
            }
            float randomValue = Random.Range(0, range_Max);

            float pointer = 0;
            for (int k = 0; k < attackGroup[i].probability.Length; k++)
            {
                pointer += attackGroup[i].probability[k];

                if (pointer > randomValue)
                {
                    attackCommandSequenceList.Add(attackGroup[i].attackSequenceIndex[k]);
                    break;
                }
            }
        }
    }

    // 载入攻击序列
    private void LoadAttackSequence() {
        inAttackSequence = true;

        currentAttackSequence.Clear();
        time_CurrentAttackSequence = 0;
        timer_CurrentAttackSequence = 0;

        time_CurrentAttackSequenceStart.Clear();
        time_CurrentAttackSequencePerfectBlockEnd.Clear();
        time_CurrentAttackSequenceStartAttack.Clear();
        time_CurrentAttackSequenceEnd.Clear();

        // 获取攻击指令列表首个指令并删除指令
        int index = attackCommandSequenceList[0];
        attackCommandSequenceList.RemoveAt(0);

        // 载入攻击类型和攻击序列线
        for (int i = 0; i < attackSequence[index].attackType.Length; i++) {
            int type = attackSequence[index].attackType[i];
            currentAttackSequence.Add(type);

            // 生成攻击序列特殊分段点(起始点)
            time_CurrentAttackSequenceStart.Add(time_CurrentAttackSequence);
            // 生成攻击序列特殊分段点(攻击开始点)
            time_CurrentAttackSequenceStartAttack.Add(attackType[type].time_BefroeAttack + time_CurrentAttackSequence);
            // 生成攻击序列特殊分段点(完美格挡点)
            time_CurrentAttackSequencePerfectBlockEnd.Add(attackType[type].time_BefroeAttack + attackType[type].time_PerfectBlock + time_CurrentAttackSequence);
            // 生成攻击序列特殊分段点 (攻击结束点)
            time_CurrentAttackSequenceEnd.Add(time_CurrentAttackSequence + attackType[attackSequence[index].attackType[i]].time_BefroeAttack + attackType[attackSequence[index].attackType[i]].time_Attack);

            // 生成攻击序列时间线
            time_CurrentAttackSequence += attackType[attackSequence[index].attackType[i]].time_BefroeAttack + attackType[attackSequence[index].attackType[i]].time_Attack;

        }
    }

    // 玩家格挡
    public void PlayerBlock() {
        // 正在执行攻击序列中
        if (inAttackSequence && !finishedJudgement)
        {
            blocked = true;
            player.SendMessage("ReduceEnergy", 1,SendMessageOptions.DontRequireReceiver); // 固定减少体力值
        }
    }

    // 检测玩家QTE决定操作 0->左 1->右 2->全屏
    public void CheckPlayerOperation(int _dir) {
        if (finishedJudgement) return;

        // 前摇期间格党
        if(!inAttackingJudgment && !inPerfectBlockJudgement){
            if (blocked == true) {
                Debug.Log("玩家在前摇期间内格挡");
                currentPlayerJudgement = Judgement.Block;
                ExecutePlayerJudgement(_dir);
                return;
            }
        }

        // 攻击期间内操作判定
        switch (_dir) {
            case 0: {
                    // 玩家操作检测
                    if (inAttackingJudgment)
                    {
                        // 完美时间段判定
                        if (inPerfectBlockJudgement)
                        {
                            if (blocked)
                            {
                                if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.PerfectBlock ? currentPlayerJudgement : Judgement.PerfectBlock;
                                }
                                else if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Block ? currentPlayerJudgement : Judgement.Block;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.HittedBlock ? currentPlayerJudgement : Judgement.HittedBlock;
                                }
                            }
                            else
                            {
                                if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                                else if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Roll ? currentPlayerJudgement : Judgement.Roll;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                            }

                            break;
                        }
                        // 攻击时间段中其他时间段判定
                        else
                        {
                            if (blocked)
                            {
                                if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement >Judgement.HittedBlock ? currentPlayerJudgement : Judgement.HittedBlock;
                                }
                                else if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Block ? currentPlayerJudgement : Judgement.Block;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.HittedBlock ? currentPlayerJudgement : Judgement.HittedBlock;
                                }
                            }
                            else
                            {
                                if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                                else if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Roll ? currentPlayerJudgement : Judgement.Roll;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                            }
                        }

                        
                    }
                    break;
                }
            case 1: {
                    // 玩家操作检测
                    if (inAttackingJudgment)
                    {
                        // 完美时间段判定
                        if (inPerfectBlockJudgement)
                        {
                            if (blocked)
                            {
                                if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.PerfectBlock ? currentPlayerJudgement : Judgement.PerfectBlock;
                                }
                                else if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Block ? currentPlayerJudgement : Judgement.Block;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.HittedBlock ? currentPlayerJudgement : Judgement.HittedBlock;
                                }
                            }
                            else
                            {
                                if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                                else if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Roll ? currentPlayerJudgement : Judgement.Roll;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                            }

                            break;
                        }
                        // 攻击时间段中其他时间段判定
                        else
                        {
                            if (blocked)
                            {
                                if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.HittedBlock ? currentPlayerJudgement : Judgement.HittedBlock;
                                }
                                else if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Block ? currentPlayerJudgement : Judgement.Block;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.HittedBlock ? currentPlayerJudgement : Judgement.HittedBlock;
                                }
                            }
                            else
                            {
                                if (joystick.GetVector().x > 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                                else if (joystick.GetVector().x < 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Roll ? currentPlayerJudgement : Judgement.Roll;
                                }
                                else if (joystick.GetVector().x == 0)
                                {
                                    currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                                }
                            }
                        }


                    }
                    break;
                }
            case 2: {
                    // 玩家操作检测
                    if (inAttackingJudgment)
                    {
                        // 完美时间段判定
                        if (inPerfectBlockJudgement)
                        {
                            if (blocked)
                            {
                                currentPlayerJudgement = currentPlayerJudgement > Judgement.PerfectBlock ? currentPlayerJudgement : Judgement.PerfectBlock;
                            }
                            else
                            {
                                currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                            }

                            break;
                        }
                        // 攻击时间段中其他时间段判定
                        else
                        {
                            if (blocked)
                            {
                                currentPlayerJudgement = currentPlayerJudgement > Judgement.HittedBlock ? currentPlayerJudgement : Judgement.HittedBlock;
                            }
                            else
                            {
                                currentPlayerJudgement = currentPlayerJudgement > Judgement.Hitted ? currentPlayerJudgement : Judgement.Hitted;
                            }
                        }


                    }
                    break;
                }
            default:break;
        }
    }

    // 执行玩家操作判定结果并锁定判定 0->左 1->右 2->全屏
    public void ExecutePlayerJudgement(int _dir) {
        finishedJudgement = true;

        switch (currentPlayerJudgement) {
            case Judgement.None: {
                    Debug.Log("None");
                    break;
                }
            case Judgement.Roll: {
                    Debug.Log("玩家翻滚");

                    GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/QTE_Feedback"));
                    go.GetComponent<QTE_Feedback>().SetSprite(0, _dir);
                    break;
                }
            case Judgement.Block: {
                    Debug.Log("玩家普通格挡");
                    player.SendMessage("AddPressurePoint", pressurePointIncrementWhenHitted, SendMessageOptions.DontRequireReceiver);

                    GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/QTE_Feedback"));
                    go.GetComponent<QTE_Feedback>().SetSprite(1);
                    break;
                }
            case Judgement.Hitted: {
                    Debug.Log("玩家被击中 减少逃生点 : " + attackType[_dir].damage);
                    player.SendMessage("AddPressurePoint", pressurePointIncrementWhenHitted, SendMessageOptions.DontRequireReceiver);
                    player.SendMessage("ReduceEscapePoint", attackType[_dir].damage,SendMessageOptions.DontRequireReceiver);

                    GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/QTE_Feedback"));
                    go.GetComponent<QTE_Feedback>().SetSprite(2);
                    break;
                }
            case Judgement.HittedBlock: {
                    Debug.Log("玩家格挡受击 额外减少体力 : " + attackType[_dir].energyExpendIfBlock);
                    player.SendMessage("AddPressurePoint", pressurePointIncrementWhenHitted, SendMessageOptions.DontRequireReceiver);
                    player.SendMessage("ReduceEnergy", attackType[_dir].energyExpendIfBlock, SendMessageOptions.DontRequireReceiver);

                    GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/QTE_Feedback"));
                    go.GetComponent<QTE_Feedback>().SetSprite(3);
                    break;
                }
            case Judgement.PerfectBlock: {
                    Debug.Log("玩家完美格挡 增加逃生点 : 20");
                    player.SendMessage("AddEscapePoint", 20, SendMessageOptions.DontRequireReceiver);

                    GameObject go = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/QTE_Feedback"));
                    go.GetComponent<QTE_Feedback>().SetSprite(4);
                    break;
                }
            default:break;
        }
    }
}