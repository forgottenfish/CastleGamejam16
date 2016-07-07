﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TimelineView : MonoBehaviour
{
	public class Context
	{
		public GameModel model;
		public Timeline timeline;
		public VisualPrefabLoader loader;
	}

	[SerializeField]
	DangerView m_dangerPrefab;

	public Timeline Timeline { get { return m_context.timeline; } }
	public GameModel Model { get { return m_context.model; } }
	public Environment Environment { get; private set; }

	List<DangerView> m_activeDangers = new List<DangerView>();
	Context m_context;

	public void Init(Context context)
	{
		m_context = context;

		var environmentPrefab = m_context.loader.GetEnvironmentPrefab(Timeline.TimelineType);

		var environment = (GameObject)Instantiate(environmentPrefab, transform.position, transform.rotation);
		environment.transform.parent = transform;
		Environment = environment.GetComponent<Environment>();

		Timeline.OnDangerAdded += HandleDangerAdded;

		foreach (var danger in Timeline.m_dangers)
		{
			var dangerView = CreateDangerView(danger);
			m_activeDangers.Add(dangerView);
		}
	}

	private void HandleDangerAdded(Danger danger)
	{
		CreateDangerView(danger);
	}

	DangerView CreateDangerView(Danger danger)
	{
		var dangerView = (DangerView)Instantiate(m_dangerPrefab, transform.position, transform.rotation);
		dangerView.transform.parent = transform;

		var dangerContext = new DangerView.Context()
		{
			model = Model,
			timeline = Timeline,
			danger = danger,
			loader = m_context.loader
		};

		dangerView.Init(dangerContext);
		dangerView.OnDestroy += ViewDestroyedHandler;

		return dangerView;
	}

	void ViewDestroyedHandler(DangerView dangerView)
	{
		m_activeDangers.Remove(dangerView);
	}
}