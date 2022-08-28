using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float Health = 100;
    public float Stamina = 100;

    public float HealthReducePerHit = 10;
    public float StaminaReducePerHit = 10;

    public float HealthncreaseMultiplier = 0.5f;
    public float StaminaIncreaseMultiplier = 5;

    public bool CanIncraseStamina = true;
    public bool CanIncraseHealth = false;

    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private float _sprintSpeedMultiplier = 2;
    [SerializeField] private float _jumpHeight = 3;

    [SerializeField] private bool _isGrounded = true;
    public bool IsBlocking = true;

    private int _punchCombo = 0;

    private CharacterController _characterController;
    private float _turnSpeed = 360;
    private Vector3 _input;

    private float _gravity = -9.81f;

    public Animator _animator;

    public GameObject RightHand;

    public GameObject CurrentObjectOnHand;


    private void Start()
    {
        IsBlocking = false;
        _characterController = GetComponent<CharacterController>();  
        _animator = GetComponent<Animator>();
    }

    private bool _isDead;
    public IEnumerator KillPlayer()
    {
        _isDead = true;
        _animator.SetTrigger("Died");
        SoundManager.Instance.PlaySFX(SoundManager.Instance.DEAD);
        yield return new WaitForSeconds(2f);

        Time.timeScale = 0;
        UIController.Instance.EnableGameOverScreen(true);
    }

    private void Update()
    {
        if (_isDead) return;

        CheckForGrounded();
        GetInput();
        Look();
        Punch();
        Block();
        Move();
        CheckForJump();
        CheckForPickUp();
        CheckForItemDrop();

        if (CanIncraseStamina)
            IncreaseStaminaOvertime();

        if(CanIncraseHealth)
            IncreaseHealthOvertime();

        if (Health <= 0)
            StartCoroutine(KillPlayer());
    }


    private void GetInput()
    {
        _input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
    }

    private void Look()
    {
        if (_input == Vector3.zero) return;

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Left_Jab") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Right_Jab") || IsBlocking
    || _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Prop_Meele"))
            return;

        Quaternion rot = Quaternion.LookRotation(_input.ToIso(), Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, _turnSpeed * Time.deltaTime);
    }

    private void Move()
    {
        float inputMagnitude = _input.normalized.magnitude;

        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Left_Jab") || _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Right_Jab") || IsBlocking
    || _animator.GetCurrentAnimatorStateInfo(0).IsName("Player_Prop_Meele"))
            inputMagnitude = 0;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            inputMagnitude *= _sprintSpeedMultiplier;
        }

        _animator.SetFloat("MovementBlend", inputMagnitude);

        _characterController.Move(transform.forward * inputMagnitude * _moveSpeed * Time.deltaTime);
        
    }


    public Vector3 playerVelocity;
    private void CheckForJump()
    {        
        if (_isGrounded && playerVelocity.y < -1f)
            playerVelocity.y = 0;

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(_jumpHeight * -3f * _gravity);
            _animator.SetTrigger("Jump");
        }

        playerVelocity.y += _gravity * 10 * Time.deltaTime;
        _characterController.Move(playerVelocity * Time.deltaTime);
    }

    private bool _prevGrounded = true;
    private void CheckForGrounded()
    {
        _prevGrounded = _isGrounded;

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.3f))
        {
            if (hit.transform.CompareTag("Ground"))
            {
                _isGrounded = true;
            }
        }
        else
        {
            _isGrounded = false;
        }

        if (_prevGrounded != _isGrounded)
            _animator.SetTrigger("JumpLanded");
    }

    private void Punch()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CurrentObjectOnHand == null)
            {
                switch (_punchCombo)
                {
                    case 0:
                        _punchCombo = 1;
                        _animator.SetTrigger("Punch1");
                        break;
                    case 1:
                        _punchCombo = 0;
                        _animator.SetTrigger("Punch2");
                        break;
                }
            }
            else
            {
                _animator.SetTrigger("WeaponHit");
            }
        }
    }

    private void Block()
    {
        if (Input.GetMouseButtonDown(1))
        {
            _animator.SetBool("IsBlocking", true);
            IsBlocking = true;
        }
        else if(Input.GetMouseButtonUp(1))
        {
            _animator.SetBool("IsBlocking", false);
            IsBlocking = false;
        }
    }

    private void CheckForPickUp()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if(LevelManager.Instance.AvalaiblePickupObjects.Count > 0)
            {
                if (CurrentObjectOnHand != null)
                {
                    CurrentObjectOnHand.transform.parent = null;
                    CurrentObjectOnHand.GetComponent<CapsuleCollider>().enabled = true;
                    CurrentObjectOnHand.GetComponent<Rigidbody>().isKinematic = false;
                    CurrentObjectOnHand.GetComponent<PickableObject>().CanPickup = true;
                }

                GameObject obj = LevelManager.Instance.AvalaiblePickupObjects[0];
                LevelManager.Instance.AvalaiblePickupObjects.RemoveAt(0);

                obj.GetComponent<CapsuleCollider>().enabled = false;
                obj.GetComponent<Rigidbody>().isKinematic= true;
                obj.GetComponent<PickableObject>().CanPickup = false;
                obj.transform.SetParent(RightHand.transform);
                obj.transform.localPosition = obj.GetComponent<PickableObject>().PostionOffset;
                obj.transform.localRotation = Quaternion.Euler(obj.GetComponent<PickableObject>().ObjectRotation);

                CurrentObjectOnHand = obj;
                SoundManager.Instance.PlaySFX(SoundManager.Instance.PICKUPITEM);

                if (CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.STOP)
                {
                    CurrentObjectOnHand.transform.GetChild(0).gameObject.SetActive(false);
                }

                UIController.Instance.EnablePickupUI(false);
            }
        }
    }

    private void CheckForItemDrop()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (CurrentObjectOnHand != null)
            {
                CurrentObjectOnHand.transform.parent = null;
                CurrentObjectOnHand.GetComponent<CapsuleCollider>().enabled = true;
                CurrentObjectOnHand.GetComponent<Rigidbody>().isKinematic = false;
                CurrentObjectOnHand.GetComponent<PickableObject>().CanPickup = true;

                if (CurrentObjectOnHand.GetComponent<PickableObject>().PickableType == PickableType.STOP)
                {
                    CurrentObjectOnHand.transform.position = transform.position + transform.forward + (Vector3.up * 2);
                    CurrentObjectOnHand.transform.rotation = transform.rotation;

                    CurrentObjectOnHand.transform.GetChild(0).gameObject.SetActive(true);
                }

                CurrentObjectOnHand = null;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyHit"))
        {
            if (IsBlocking)
            {
                _animator.SetTrigger("DamageTakenBlock");
            }
            else
            {
                other.transform.root.GetComponent<EnemyController>().ReduceHealth(10);
                ReduceStamina(StaminaReducePerHit);
                if(Stamina <= 0)
                {
                    ReduceHealth(HealthReducePerHit);
                }
                _animator.SetTrigger("DamageTaken");
            }
            SoundManager.Instance.PlaySFX(SoundManager.Instance.TAKEDAMAGE);
        }
    }

    private float staminaTimeElapsed = 0;
    private void IncreaseStaminaOvertime()
    {
        staminaTimeElapsed += Time.deltaTime;

        if(staminaTimeElapsed >= StaminaIncreaseMultiplier)
        {
            staminaTimeElapsed %= StaminaIncreaseMultiplier;
            AddStamina(1);
        }
    }

    private float healthTimeElapsed = 0;
    private void IncreaseHealthOvertime()
    {
        healthTimeElapsed += Time.deltaTime;

        if (healthTimeElapsed >= HealthncreaseMultiplier)
        {
            healthTimeElapsed %= HealthncreaseMultiplier;
            AddHealth(1);
        }
    }

    public void AddHealth(float value)
    {
        Health = Mathf.Clamp(Health + value, 0, 100);
    }

    public void ReduceHealth(float value)
    {
        Health = Mathf.Clamp(Health - value, 0, 100);
    }

    public void AddStamina(float value)
    {
        Stamina = Mathf.Clamp(Stamina + value, 0, 100);
    }

    public void ReduceStamina(float value)
    {
        Stamina = Mathf.Clamp(Stamina - value, 0, 100);
    }
}
