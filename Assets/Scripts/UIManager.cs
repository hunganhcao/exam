using ScriptableObjectArchitecture;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[SerializeField] GameObject popupWin;
	[SerializeField] GameObject popupRevive;
	[SerializeField] GameObject popupLose;
	[SerializeField] BoolVariable isWin;
	[SerializeField] BoolVariable isLose;
	[SerializeField] TextMeshProUGUI countTxt;
	[SerializeField] Button reviveBtn;
	[SerializeField] Button loseBtn;
	[SerializeField] Button winBtn;
	private bool _isCount;
	private float _countDown;
	private int _timeCount;

	private void Awake()
	{
		isWin.AddListener(ShowWin);
		isLose.AddListener(ShowRevive);
		_isCount = false;
		_timeCount = 10;
		_countDown=_timeCount;
		loseBtn.onClick.AddListener(HideLoseWin);
		winBtn.onClick.AddListener(HideLoseWin);
		reviveBtn.onClick.AddListener(Revive);
	}

	private void Revive()
	{
		isLose.Value = false;
	}

	private void HideLoseWin()
	{
		isWin.Value = false;
		SceneManager.LoadScene(0);
	}

	private void OnDestroy()
	{
		isWin.RemoveListener(ShowWin);
		isLose.RemoveListener(ShowRevive);
		loseBtn.onClick.RemoveListener(HideLoseWin);
		winBtn.onClick.RemoveListener(HideLoseWin);
		reviveBtn.onClick.RemoveListener(Revive);
	}
	private void Update()
	{
		if (_isCount)
		{
			if (_countDown > 0)
			{
				_countDown-=Time.deltaTime;
				countTxt.text = _countDown.ToString("F0");
			}
			else
			{
				isLose.Value = false;
				ShowLose();
				_countDown = _timeCount;
				_isCount = false;
			}
		}
			
	}
	private void ShowWin()
	{
		popupWin.SetActive(isWin.Value);
	}
	private void ShowRevive()
	{
		popupRevive.SetActive(isLose.Value);
		_countDown = _timeCount;
		_isCount =true;
		
	}
	private void ShowLose()
	{
		popupLose.SetActive(true);
	}

}
