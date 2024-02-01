using ScriptableObjectArchitecture;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
	[SerializeField] private float speedJump;
	[SerializeField] private float speedDown;
	private Rigidbody _rb;
	[ShowInInspector]
	private bool _clicking;
	[SerializeField] BoolVariable isWin;
	[SerializeField] BoolVariable isLose;
	private bool _isEnd;
	private void Awake()
	{
		isLose.AddListener(Revive);
		_rb = GetComponent<Rigidbody>();
	}

	private void Revive()
	{
		_clicking = false;
		if (_rb != null)
		{
			_rb.velocity = Vector3.up * speedJump;
		}

		_isEnd = false;
	}

	private void Start()
	{
		_clicking = false;
		_isEnd = false;
	}

	private void Update()
	{
		Clicking();
	}
	private void Clicking()
	{
		if(_isEnd) { return; }
		if (Input.GetMouseButtonDown(0))
		{
			_clicking = true;
		}
		if (Input.GetMouseButtonUp(0))
		{
			_clicking = false;
		}
		if (_clicking)
		{
			_rb.velocity = Vector3.up * -speedDown;
		}
	}
	private void OnCollisionEnter(Collision other)
	{
		if (_isEnd) { return; }
		if (_clicking)
			{
				if (other.collider.gameObject.CompareTag("Win"))
				{
					Debug.Log("WIN");
					isWin.Value = true;
					_isEnd=true;
				}
				if (other.collider.gameObject.CompareTag("Bad"))
				{
					Debug.Log("LOSE");
					isLose.Value = true;
					_isEnd=true;
				}
				else if (other.collider.gameObject.CompareTag("Good"))
				{
					Destroy(other.collider.gameObject);
				}
			}
			else
			{
				if (other.collider.gameObject.CompareTag("Bad"))
				{
					Destroy(other.collider.gameObject);
				}
				_rb.velocity = Vector3.up * speedJump;
			}
		
	}
}
