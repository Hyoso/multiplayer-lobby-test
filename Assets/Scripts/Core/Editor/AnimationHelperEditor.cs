using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CustomEditor(typeof(AnimationHelper), true)]
public class AnimationHelperEditor : Editor
{
	private string animations = "Click to generate";

	public override void OnInspectorGUI()
	{
		AnimationHelper element = (AnimationHelper)target;

		DrawDefaultInspector();

		if (GUILayout.Button("Read Animations"))
		{
			GetAnimations();
		}
		
		if (GUILayout.Button("Generate Code"))
		{
			List<AnimationHelper.AnimState> animList = element.AnimStatesList;
			animations = "";
			foreach (var item in animList)
			{
				string animName = item.animationName;
				string animConstd = animName.ConvertToConstCase();
				string finalAnimName = "private const string ANIM_{0} = \"{1}\";\n";
				animations += string.Format(finalAnimName, animConstd, animName);
			}
		}

		if (GUILayout.Button("Copy Code"))
		{
			TextEditor ed = new TextEditor();
			ed.text = animations;
			ed.SelectAll();
			ed.Copy();
		}

		EditorGUILayout.TextArea(animations);
	}

	private void GetAnimations()
	{
		AnimationHelper element = (AnimationHelper)target;
		Animator animator = element.Animator;
		AnimatorController animController = (AnimatorController)animator.runtimeAnimatorController;
		if (animController == null)
		{
			AnimatorOverrideController overrideController;
			overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
			animController = (AnimatorController)overrideController.runtimeAnimatorController;
		}
		
		if (animController != null)
		{
			AnimatorControllerLayer[] layers = animController.layers;
			foreach (var item in layers)
			{
				AnimatorStateMachine sm = item.stateMachine;
				ChildAnimatorState[] states = sm.states;
				for (int i = 0; i < states.Length; i++)
				{
					element.AddState(states[i].state.name);
				}
			}

			ReloadPreviewInstances();
		}

		EditorUtility.SetDirty(element);
	}
}
