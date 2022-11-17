using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
	private Canvas canvas;
	private RectTransform rectTransform;
	private CanvasGroup canvasGroup;
	
	public int number;
	private GameControler gameControler;
	//private bool isDragging = false;
	private bool isOverDropZone = false;
	private bool isOverDefausseZone = false;
	private bool isDraggable = true;
	private GameObject dropZone;
	private GameObject handZone;
	private GameObject defaussezone;
	private Vector2 startPosition;
	[SerializeField] Text cardText;
	[SerializeField] Image image;

	public void SetImage(Sprite s)
    {
		this.image.sprite = s;
    }

	public Sprite getSprite()
	{
		return this.image.sprite;
	}

	public void Awake()
    {
		gameControler = GameObject.Find("Game Controler - Lancer Partie").GetComponent<GameControler>();
		handZone = gameControler.handzone;
		defaussezone = gameControler.defausseZone;
		dropZone = gameControler.drop_zone;
		canvas = GameObject.Find("Game Controler - Lancer Partie").transform.GetChild(3).GetComponent<Canvas>();
		Debug.Log(canvas.scaleFactor.ToString());
		canvasGroup = GetComponent<CanvasGroup>();
		rectTransform = GetComponent<RectTransform>();
	}

	public void OnBeginDrag(PointerEventData eventData)
    {
		startPosition = transform.localPosition;
		gameControler.drop_zone.GetComponent<Image>().color = new Color32(46,36,21,71);
		gameControler.defausseZone.GetComponent<Image>().color = new Color32(135,31,18,109);
		if(isDraggable){
			Debug.Log("begin drag");
			canvasGroup.blocksRaycasts = false;
			//isDragging = true;
		}
		else
		{
			transform.position = startPosition;
		}
		//isDragging = false;
    }

	public void OnDrag(PointerEventData eventData)
    {
		Debug.Log("On drag");
		canvasGroup.alpha = .6f;

		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

	public void OnPointerDown(PointerEventData eventData)
    {
		Debug.Log("pointer down");
    }

	public void OnEndDrag(PointerEventData eventData)
    {
		float Distance = Vector2.Distance(startPosition, transform.position);
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
		gameControler.drop_zone.GetComponent<Image>().color = new Color32(46,36,21,32);
		gameControler.defausseZone.GetComponent<Image>().color = new Color32(135,31,18,150);

		if(isOverDropZone)
		{
			Debug.Log("drop zone");
			gameControler.set_indexcarte(this.number);
			gameControler.action_joueur();
			// Action joueur
		  //  transform.SetParent(dropZone.transform, false);
		   // transform.localPosition = gameControler.carteJouee.transform.localPosition;
		
		}
		else if (isOverDefausseZone)
        {
			Debug.Log("defausse zone");
			gameControler.set_indexcarte(this.number);
			gameControler.defausser();
			// Action joueur
		    //transform.SetParent(defaussezone.transform, false);
		   // transform.localPosition = Vector3.zero;
        }
		else
		{
			Debug.Log("outside of zone");
			gameControler.players[0].GetComponent<Joueur>().Mise_a_jour_carte();
		}
		//isDragging = false;
    }

	// public void SetText(string str)
	// {
	// 	cardText.text = str;
	// }

	public void setDraggable(bool i)
	{
		this.isDraggable = i;
	}

	public void OnDropZone(GameObject zone)
    {
		isOverDropZone = true;
		dropZone = zone;
		transform.SetParent(dropZone.transform, false);
		rectTransform.anchoredPosition = gameControler.carteJouee.GetComponent<RectTransform>().anchoredPosition;
		setDraggable(false);
    }

	public void OnDefausseZone()
    {
		isOverDefausseZone = true;
    }
}
