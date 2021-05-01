using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.IO;
using JsonReaderYugi;

public class CardMaker : MonoBehaviour
{

    public GameObject CardTemplate;
    public InputField CardNameInput;
    public InputField CardDescriptionInput;
    public InputField CardAtkInput;
    public InputField CardDefInput;
    public Text CharCount;
    private string cardName;
    private string cardDescription;
    private int cardAtk;
    private int cardDef;
    static List<JsonReaderYugi.Card> cardList;
    static List<Sprite> bigCardsSprites;
    static List<Sprite> smallCardsSprites;
    void Start()
    {
        CharCount.text = "";
        cardList = LoadData.cardList;
        bigCardsSprites = LoadData.bigCardsSprites;
        smallCardsSprites = LoadData.smallCardsSprites;
        Debug.Log("cardList: "+cardList.Count);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReadCardName(string s) {
        CharCount.text = s.Length + "/23";
        if (s.Length <= 23)
        {
            
            cardName = s;
            Debug.Log(cardName);
            Text CardName = CardTemplate.transform.Find("CardName").gameObject.GetComponent<UnityEngine.UI.Text>();
            CardName.text = cardName;
        }
        
    }

    public void ReadCardDescription(string s) {
        CharCount.text = s.Length + "/107";
        if (s.Length <= 107)
        {
            cardDescription = s;
            Debug.Log(cardDescription);
            Text CardDescription = CardTemplate.transform.Find("CardDescription").gameObject.GetComponent<UnityEngine.UI.Text>();
            CardDescription.text = cardDescription;
        }
    }

    public void ReadCardAtk(string s) {
        CharCount.text = s.Length + "/6";
        if (s.Length <= 6)
        {
            if (Int32.TryParse(s, out int j))
            {
                cardAtk = j;
                Debug.Log(cardAtk);
                Text CardAtk = CardTemplate.transform.Find("CardAtk").gameObject.GetComponent<UnityEngine.UI.Text>();
                CardAtk.text = s;
            }
            else
            {
                CardAtkInput.Select();
                CardAtkInput.text = "";
            }
        }
        
    }

    public void ReadCardDef(string s) {
        CharCount.text = s.Length + "/6";
        if (s.Length <= 6)
        {
            if (Int32.TryParse(s, out int j))
            {
                cardDef = j;
                Debug.Log(cardDef);
                Text CardDef = CardTemplate.transform.Find("CardDef").gameObject.GetComponent<UnityEngine.UI.Text>();
                CardDef.text = s;
            }
            else
            {
                CardDefInput.Select();
                CardDefInput.text = "";
            }
        }
    }

    public void SelectImageButton() {

        string path = EditorUtility.OpenFilePanel("Seleccione una imagen", "", "jpg");
        Image CardArt = CardTemplate.transform.Find("CardArt").gameObject.GetComponent<UnityEngine.UI.Image>();
        CardArt.sprite = LoadNewSprite(path);
    }

    public void SaveCardButton()
    {
        if (CardNameInput.text != "" && CardDescriptionInput.text != "" && CardAtkInput.text != "" && CardDefInput.text != "")
            StartCoroutine(ExportCard());
        else
            CharCount.text = "Ningun campo de texto debe estar vacio";
    }

    public Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f) {

        // Load a PNG or JPG image from disk to a Texture2D, assign this texture to a new sprite and return its reference
        Sprite NewSprite;

        Texture2D SpriteTexture = LoadTexture(FilePath);
        NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);
        
        return NewSprite;
    }

    public Texture2D LoadTexture(string FilePath) {

        // Load a PNG or JPG file from disk to a Texture2D
        // Returns null if load fails

        Texture2D Tex2D;
        byte[] FileData;

        if (File.Exists(FilePath)) {
            FileData = File.ReadAllBytes(FilePath);
            Tex2D = new Texture2D(2, 2);           // Create new "empty" texture
            if (Tex2D.LoadImage(FileData))           // Load the imagedata into the texture (size is set automatically)
                return Tex2D;                 // If data = readable -> return texture
        }
        return null;                     // Return null if load failed
    }

    WaitForSeconds waitTime = new WaitForSeconds(0.1F);
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator ExportCard()
    {
        yield return waitTime;
        yield return frameEnd;
        int width = 475;
        int height = 670;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        Rect sel = new Rect();
        sel.width = width;
        sel.height = height;
        sel.x = CardTemplate.transform.position.x - 235;
        sel.y = CardTemplate.transform.position.y - 335;

        tex.ReadPixels(sel, 0, 0);

        byte[] bytes = tex.EncodeToJPG();

        SaveCardCreated(bytes);
       // CharCount.text = "Carta guardada";
        /*try
        {
            
            
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
            CharCount.text = "Error al guardar la carta";
        }*/
        

        
    }

    public void SaveCardCreated(byte[] bytes)
    {
        Card card = new Card();
        card.Name = cardName;
        card.Desc = cardDescription;
        card.Atk = cardAtk;
        card.Def = cardDef;
        card.Id = generateID().ToString();
        File.WriteAllBytes("Assets/Resources/Cards/" + card.Id + ".jpg", bytes);
        File.WriteAllBytes("Assets/Resources/SmallCards/" + card.Id + ".jpg", bytes);
        cardList.Add(card);
        bigCardsSprites.Add(LoadNewSprite("Assets/Resources/Cards/" + card.Id + ".jpg"));
        smallCardsSprites.Add(LoadNewSprite("Assets/Resources/SmallCards/" + card.Id + ".jpg"));
    }

    private int generateID()
    {
        System.Random r = new System.Random();
        int id = r.Next(1, 9999999);
        foreach(Card c in cardList)
        {
            if (Int32.Parse(c.Id) == id)
                return generateID();
        }
        return id;
    }

}