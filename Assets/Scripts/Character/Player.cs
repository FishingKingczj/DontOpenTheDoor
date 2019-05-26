using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MoveObject
{
	
	private const float DEFAULT_MAXENERGY = 100.0f;
	[Header("Energy varible")]
	public float energy_Current = DEFAULT_MAXENERGY;

	public float energyConsumption_Idle =  0.33f;
	public float energyConsumption_Walk =  0.33f;
	public float energyConsumption_Rush =  5.0f;

    //活力条
	public Slider energy_Slider;

    [Header("Interact varible")]
    public float interaction_Range = 1.2f;

    private const float DEFAULT_SPEED = 1f;
    private const float RUSH_SPEED = DEFAULT_SPEED * 1.5f;

	void Start(){
        CircleCollider2D box = gameObject.AddComponent<CircleCollider2D>();
        box.radius = interaction_Range;
        box.isTrigger = true;

        energy_Slider = GameObject.Find("slider_Energy").GetComponent<Slider>();
	}

    void FixedUpdate()
    {
		if(energy_Current > 0)
		{
			PlayerRush();
			PlayerMove();
            PlayerInteract();
		}
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        //Debug.Log("你碰到了" + collider.name.ToString());
    }

    // 接触结束
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider != null)
        {
            //Debug.Log("你离开了" + collider.name.ToString());
        }
    }

    // 控制人物移动
    private void PlayerMove()
    {
        float dx = Input.GetAxis("Horizontal");
        float dy = Input.GetAxis("Vertical");
        Vector3 vector = new Vector3(dx, dy);

		// 检测是否移动 (玩家体力消耗)
		if(vector != Vector3.zero){
			// 检测奔跑速度
			if(GetSpeed() == DEFAULT_SPEED)
				energy_Current -= energyConsumption_Walk * Time.deltaTime;
			else
				energy_Current -= energyConsumption_Rush * Time.deltaTime;
		}else{
			energy_Current -= energyConsumption_Idle * Time.deltaTime;
		}
		energy_Current = Mathf.Clamp(energy_Current,0,DEFAULT_MAXENERGY);
		energy_Slider.value = energy_Current;

        Move(vector);
    }

    // 控制人物冲刺
    private void PlayerRush()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            setSpeed(RUSH_SPEED);
        }
        else
        {
            setSpeed(DEFAULT_SPEED);
        }
    }

    // 控制玩家互动
    private void PlayerInteract() {
        // 如果玩家正在使用物品，屏蔽地图交互
        if (GetComponent<Player_BackPack>().inSelected || GetComponent<Player_BackPack>().inMultipleSelected)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(this.transform.position, interaction_Range);
            if (colliders.Length > 0)
            {
                Collider2D item = null;
                float dis = 0x9f9f9f9f;

                for (int i = 0; i < colliders.Length; i++)
                {
                    if (!colliders[i].tag.Contains("Item"))
                        continue;
                    else
                    {
                        if (Vector2.Distance(this.gameObject.transform.position, colliders[i].transform.position) < dis)
                        {
                            dis = Vector2.Distance(this.gameObject.transform.position, colliders[i].transform.position);
                            item = colliders[i];
                        }
                    }
                }

                if (item == null) { return; }
                else item.SendMessage("Interact", this.gameObject, SendMessageOptions.DontRequireReceiver); 
            }
        }
    }

    // 增加体力
    public void AddEnergy(float _value) {
        energy_Current += _value;
    }
}
