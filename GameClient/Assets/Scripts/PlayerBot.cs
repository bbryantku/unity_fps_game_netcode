using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBot : MonoBehaviour
{
    public int id;
    public PlayerBotState state;
    public EnemyManager target;

    public Vector3 player_to_enemy;
    public Vector3 player_to_target;
    public PlayerManager player;
    public float gravity = -9.81f;
    public float patrolSpeed = .5f;
    public float chaseSpeed = 1f;
    public float detectionRange = 30f;
    public float shootRange = 20f;
    public float patrolDuration = 2f;
    public float idleDuration = 3f;

    private bool isPatrolRoutineRunning;
    
    private void Start()
    {
        string _methodName= "PlayerBot.Start()";
        try
        {
            if(Client.instance.enablePlayerBot){
                id = Client.instance.myId;
                //retreive player object for current player id
                if (GameManager.players.TryGetValue(id, out PlayerManager _player))
                {
                    player = _player;
                }
                state = PlayerBotState.patrol;
                gravity *= Time.fixedDeltaTime * Time.fixedDeltaTime;
                patrolSpeed *= Time.fixedDeltaTime;
                chaseSpeed *= Time.fixedDeltaTime;
            } //End Player Bot Behavior
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 
    }// End PlayerBot.Start()

    private void FixedUpdate()
    {
        string _methodName= "PlayerBot.FixedUpdate()";
        try
        {
            if(Client.instance.enablePlayerBot){
                switch (state)
                {
                    case PlayerBotState.idle:
                        LookForEnemy();
                        break;
                    case PlayerBotState.patrol:
                        if (!LookForEnemy())
                        {
                            Patrol();
                        }
                        break;
                    case PlayerBotState.chase:
                        Chase();
                        break;
                    case PlayerBotState.attack:
                        Attack();
                        break;
                    default:
                        break;
                }
            } //End Player Bot Behavior
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 
    } // End PlayerBot.FixedUpdate()

    private bool LookForEnemy()
    {
        string _methodName= "PlayerBot.LookForEnemy()";
        try
        {
            foreach (EnemyManager _enemy in GameManager.enemies.Values)
            {
                if (_enemy.transform != null) // changed from shootOrign to transform
                {
                    player_to_enemy = _enemy.transform.position - transform.position;
                    if (target !=null)
                    {
                        if(player_to_enemy.magnitude <= player_to_target.magnitude)
                        {
                            target=_enemy;
                        }
                    }
                    else
                    {
                        target=_enemy;
                    }
                    FaceTarget();
                    //Move();
                    if (isPatrolRoutineRunning)
                    {
                        isPatrolRoutineRunning = false;
                        StopCoroutine(StartPatrol());
                    }
                    state = PlayerBotState.chase;
                    return true;                                  
                }
            }
            return false;
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
            return false;
        } 
    } //End PlayerBot.LookForEnemy()

    private void Patrol()
    {
        string _methodName= "PlayerBot.Patrol()";
        try
        {

            if (!isPatrolRoutineRunning)
            {
                StartCoroutine(StartPatrol());
            }

        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        } 
    } //End PlayerBot.Patrol()

    private IEnumerator StartPatrol()
    {
        isPatrolRoutineRunning = true;
        if(!LookForEnemy())
        {
            //yield return new WaitForSeconds(patrolDuration);
            InitialMovement();
        
            state = PlayerBotState.idle;
            yield return new WaitForSeconds(idleDuration);

            state = PlayerBotState.patrol;
            isPatrolRoutineRunning = false;
        }
        else
        {
            isPatrolRoutineRunning = false;
        }
    } //End PlayerBot.StartPatrol()

    private void Chase()
    {
        string _methodName= "PlayerBot.Chase()";
        try
        {
            if (target!=null)
            {
                FaceTarget();
                Move();
                if (player_to_target.magnitude <= shootRange)
                {
                    state = PlayerBotState.attack;
                }    
            }
            else
            {
                state = PlayerBotState.patrol;
            }
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } // End PlayerBot.Chase()

    private void Attack()
    {
        string _methodName= "PlayerBot.Attack()";
        try
        {
            if (target!=null)
            {
                FaceTarget();  
            
                if (player_to_target.magnitude <= shootRange)
                {
                    ClientSend.PlayerShoot(player_to_target);
                }
            }
            else
            {
                state = PlayerBotState.patrol;
            }
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End PlayerBot.Attack()

    private void Move()
    {
        string _methodName= "PlayerBot.Move()";
        try
        {
        
            bool[] _inputs = new bool[]
            {
                true,
                false,
                false,
                false,
                false
            };

            ClientSend.PlayerMovement(_inputs);
        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End PlayerBot.Move()

    private void FaceTarget()
    {
        string _methodName= "PlayerBot.FaceTarget()";
        try
        {

            if (target != null)
            {
                // changed from shootOrign to transform
                player_to_target = (target.transform.position - transform.position);
                transform.LookAt(target.transform.position);               
            }

        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End PlayerBot.FaceTarget()

    private void FaceRandomDirection()
    {
        string _methodName= "PlayerBot.FaceRandomDirection()";
        try
        {

            transform.rotation=Quaternion.identity;
            transform.rotation = Quaternion.AngleAxis(UnityEngine.Random.Range(-180,180), Vector3.up);

        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    } //End PlayerBot.FaceRandomDirection()

    private void InitialMovement()
    {
        string _methodName= "PlayerBot.InitialMovement()";
        try
        {
            FaceRandomDirection();
            int count = UnityEngine.Random.Range(1,3);
            for (int i =0; i < count; i++)
            {
                Move();
                if(LookForEnemy())
                {
                    break;
                }
            }

        }
        catch (Exception e)
        {
	        Utilities.Log(Utilities.FmtLogMethodExceptionJSON(
                _methodName, "Error within "+_methodName, e));
        }
    }//End PlayerBot.InitialMovement()
    
} //End PlayerBot

public enum PlayerBotState
{
    idle,
    patrol,
    chase,
    attack,

}
