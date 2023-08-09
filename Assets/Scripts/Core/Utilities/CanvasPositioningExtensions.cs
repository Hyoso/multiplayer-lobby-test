using UnityEngine;

public static class CanvasPositioningExtensions
{
	public static Vector3 WorldToCanvasPosition(this Canvas canvas, Vector3 worldPosition, Camera camera = null)
	{
		if (camera == null)
		{
			camera = Camera.main;
		}

		Vector3 viewportPosition = camera.WorldToViewportPoint(worldPosition);
		return canvas.ViewportToCanvasPosition(viewportPosition);
	}

	public static Vector3 ScreenToCanvasPosition(this Canvas canvas, Vector3 screenPosition)
	{
		Vector3 viewportPosition = new Vector3(screenPosition.x / Screen.width,
												screenPosition.y / Screen.height, 
												0f);

		return canvas.ViewportToCanvasPosition(viewportPosition);
	}

	public static Vector3 ViewportToCanvasPosition(this Canvas canvas, Vector3 viewportPosition)
	{
		Vector3 centreBasedViewportPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0.0f);
		RectTransform rectTransform = canvas.GetComponent<RectTransform>();
		Vector2 canvasScale = rectTransform.sizeDelta;

		return Vector3.Scale(centreBasedViewportPosition, canvasScale);
	}

	public static Vector3 GetRandomVisableWorldPoint(float zPos = 0)
	{
		Camera mainCam = Camera.main;
		Vector3 randomPositionOnScreen = mainCam.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
		randomPositionOnScreen.z = zPos;

		return randomPositionOnScreen;
	}
}
