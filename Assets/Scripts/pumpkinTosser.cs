using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pumpkinTosser : MonoBehaviour {
	public float initialSpeed = 30f;
	public AudioClip homerSound;
	public AudioClip mrBurnsSound;
	public Texture crosshair;
	public Texture gamePanel;
	public Rigidbody pumpkin;

	//private bool(s)
	private bool gameOver = false;
	private bool mrBurnsSpoken = false;
	private bool won = false;
	//private float(s)
	private const float gameTotalTime = 20.0f;
	private float moveSpeed = 5f;
	private float timeRemaining = gameTotalTime;
	private float timeSinceStart;
	//private int(s)
	private int pumpkinCount = 0;
	private int blocksNotDown;
	private int blocksDown;
	//private string(s)
	private const string textColor = "#bbbbbbff";
	private const string gameOverColor = "#ccccccff";


	void Start() {
		crosshair = Resources.Load("crosshair") as Texture;
		timeSinceStart = Time.time;
	}
	// UPDATE w/ PART 4
	void Update() {
		Cursor.visible = gameOver;
		float deltaX = makeDeltaCowards("Horizontal");
		float deltaY = makeDeltaCowards("Vertical");
		transform.Translate(deltaX, deltaY, 0);
		if (!gameOver && Input.GetButtonUp("Fire1")) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			pumpkinCount++;

			if (Physics.Raycast(ray, out hit)) {
				Rigidbody pumpkinInstance = makeRigidbody(pumpkin);

				pumpkinInstance.transform.LookAt(hit.point);
				pumpkinInstance.velocity = pumpkinInstance.transform.forward * initialSpeed;
				// AudioSource.PlayClipAtPoint(homerSound, Camera.main.transform.position);
			}
		}
	}
	void OnGUI() {
		GUIStyle gs = new GUIStyle();
		gs.richText = true;
		gs.wordWrap = true;
		gs.normal.textColor = Color.white;

		Rect rectPanel = makePanel(height: gamePanel.height, widthOffset: 0);
		GUI.DrawTexture(rectPanel, gamePanel);
		if (!gameOver) {
			timeRemaining = gameTotalTime - (Time.time - timeSinceStart);
			if (timeRemaining < 0) {
				timeRemaining = 0;
			}
			updateBlocksDown();
		}
		Rect scorePanel = makePanel(x: 24, y: 101, widthOffset: 24);
		GUI.Label(scorePanel, makeLabel($"Score: {computeScore()}"), gs);

		Rect timePanel = makePanel(x: 145, y: 98, widthOffset: 145);
		GUI.Label(timePanel, makeLabel($"Left: {(int)timeRemaining}", gameOverColor));

		if (gameOver) {
			string gameResult;
			if (won) {
				gameResult = "You Win";
			} else {
				gameResult = "You Lose";
			}
			Rect gameOverPanel = makePanel(x: 32, y: 22);
			GUI.Label(gameOverPanel, makeLabel(gameResult, gameOverColor, 36));
			if (!mrBurnsSpoken) {
				// AudioSource.PlayClipAtPoint(mrBurnsSound, Camera.main.transform.position);
				mrBurnsSpoken = true;
			}
			Rect playAgainPanel = new Rect(Screen.width / 2 - 150 / 2, Screen.height / 2 - 40 / 2, 150, 40);
			if (GUI.Button(playAgainPanel, "Play Again?")) {
				SceneManager.LoadScene("WallScene");
			}
			Rect quitPanel = new Rect(Screen.width / 2 - 150 / 2, Screen.height / 2 - 40 / 2 + 50, 150, 40);
			if (GUI.Button(quitPanel, "Quit Game")) {
				Application.Quit();
			}
		} else {
			Rect infoPanel = makePanel(x: 32, y: 22, widthOffset: 32, height: 50);
			GUI.Label(infoPanel, makeLabel($"Try to knock donw all the block as fast and with as few shots as possible", textColor));
			float x = Input.mousePosition.x - (crosshair.width / 2);
			float y = Screen.height - (Input.mousePosition.y + (crosshair.height / 2));
			Rect crosshairRect = new Rect(x, y, crosshair.width, crosshair.height);
			GUI.DrawTexture(crosshairRect, crosshair);
		}
	}
	private int computeScore() {
		const float blocksOnBottomRow = 20;
		float score = ((blocksDown - blocksOnBottomRow - 1) * 1000) / (pumpkinCount + 1);
		if (gameOver) {
			score = score + Mathf.Floor(timeRemaining) * 1000;
		}
		return ((int)Mathf.Round(score));
	}
	private void updateBlocksDown() {
		const float blockNotDownThreshold = 2.5f;
		blocksDown = blocksNotDown = 0;
		MeshFilter[] meshFilters = FindObjectsOfType(typeof(MeshFilter)) as MeshFilter[];
		for (int i = 0; i < meshFilters.Length; i++) {
			bool isBlock = (meshFilters[i].mesh.vertexCount == 24);
			if (isBlock) {
				if (meshFilters[i].transform.position.y < blockNotDownThreshold) {
					blocksDown++;
				} else {
					blocksNotDown++;
				}
			}
		}
		gameOver = (blocksNotDown == 0) || (timeRemaining < 0.1f);
		won = (gameOver && blocksNotDown == 0);
	}
	// HELPER METHODS by FALLSimply
	string makeLabel(string text, string color = textColor, int size = 18) {
		return $"<size={size}><color={color}>{text}</color></size>";
	}
	Rect makePanel(int height = 40, int widthOffset = 22, int x = 10, int y = 10) {

		return new Rect(x: x, y: y, width: (gamePanel.width - widthOffset), height: height);
	}
	Rigidbody makeRigidbody(Rigidbody prefab) {
		return Instantiate(prefab, transform.position, transform.rotation) as Rigidbody;
	}
	float makeDeltaCowards(string axis) {
		return Input.GetAxis(axis) * Time.deltaTime * moveSpeed;
	}
}