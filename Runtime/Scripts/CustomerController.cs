using System;
using UnityEngine;
using UnityEngine.AI;

public class CustomerController : MonoBehaviour
{
    [SerializeField] private OrderBubbel m_orderBubbel;

    private Vector2 _walkingPoint;
    private NavMeshAgent _agent;
    private bool _walking;
    private Vector2 _startPoint;

    private Tabel _customerTabel;
    private bool _hasPayed;
    private Action _doneAction;

    public OrderBubbel OrderBubbel
    {
        get { return m_orderBubbel; }
    }

    public Action DoneAction
    {
        set { _doneAction = value; }
    }

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _startPoint = new Vector2(transform.position.x, transform.position.y);
        ShowOrder(false);
    }

    private void Update()
    {
        if (!_walking || _walkingPoint == null)
        {
            return;
        }

        Vector2 playerPosition = transform.position;
        //If the player is at the position of the mouse click or if the player isn't moving because of and obstacle
        if (Vector2.Distance(playerPosition, _walkingPoint) < 0.1f)
        {
            if (_customerTabel != null && !_customerTabel.OrderBubbel.OrderComplete)
            {
                _customerTabel.CustomerIsSitting(this);
            }
            else
            {
                ShowOrder(true);
            }
            _walking = false;

            if (_hasPayed)
            {
                Destroy(gameObject);
            }
            return;
        }

        _agent.SetDestination(_walkingPoint);
    }

    public void SetWalkingPoint(Vector3 point, Tabel tabel = null)
    {
        if (_walking)
        {
            return;
        }

        _walkingPoint = new Vector2(point.x, point.y);
        _walking = true;

        if (tabel != null)
        {
            _customerTabel = tabel;
        }
    }

    public bool Pay(LevelManager level)
    {
        if (!m_orderBubbel.OrderComplete || _walking)
        {
            return false;
        }

        //Todo Give point to player for finishing order
        _hasPayed = true;
        if (_customerTabel != null) //If this is a tabel customer the whole table payed when 
        {
            _customerTabel.TabelPayed(this);
        }

        level.AddScore();
        _doneAction?.Invoke();
        ShowOrder(false);
        SetWalkingPoint(_startPoint);
        return true;
    }

    public void hasPayed()
    {
        _hasPayed = true;
        _doneAction?.Invoke();
        SetWalkingPoint(_startPoint);
    }

    private void ShowOrder(bool show)
    {
        m_orderBubbel.gameObject.SetActive(show);
    }
}
