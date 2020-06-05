using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*****************************************************
 * CLASS:   SELECTION CONTROLLER
 *
 * INFO:    Permet de controler la sélection d'objet
 *          avec la combinaison de l'index de la main et
 *          de l'index et le pouce de la main. Ensemble,
 *          les deux gestes permettent à l'utilisateur de
 *          simuler un fusil. La sélection d'un objet 
 *          est déclenché lorsque le pouce effectue un
 *          déplacement similaire à un "trigger".
 * 
 *****************************************************/
public class SelectionController : HandDataStructure
{
    //Instance de la classe
    private static SelectionController instance = null;

    [Header("Raycast laser pointer")]
    public HandsE raycastHand = HandsE.droite;
    public FingersE raycastFinger = FingersE.index;

    [Header("Gun thumb trigger")]
    //Le temps maximum pour effectuer le geste (trigger fusil) 
    public float triggerTimer = 1.0f;
    public float timeBeforeUnselect = 2.0f;

    //GameObject materials
    [Header("Change object material on raycast hit")]
    public string selectableTag = "Selectable";
    public string shaderPath = "_OutlineTex";
    public Material defaultMaterial;
    public Material highlightMaterial;
    public Material selectedMaterial;
    public Material unselectMaterial;
    public float blinkSpeed = 0.3f;

    //Le temps effectué du geste (trigger fusil)
    private float timer = 0f;

    //Parametres du raycast de style laser rouge
    private float raycastDistance;
    private Color raycastColor;

    //Etat  de l'index
    private bool isOnlyIndexOpen = false;
    private bool wasOnlyIndexOpen = false;

    //Etat de l'index et du pouce
    private bool isOnlyIndexThumbOpen = false;
    private bool wasOnlyIndexThumbOpen = false;

    //Detection du geste 'Gun'
    private bool isGunActionDetected = false;
    private bool isStillPointing = false;
    private bool isObjectsMaterialCleared = false;

    //La liste des ID (objets) et liste des Objet sélectionnés
    private List<SelectedObject> selectedObjectsList = null;
    private List<int> selectedObjectsIDList = null;

    //Object targeted with raycast
    private int highlightObjectID = -1;
    private Transform targetedObject;
    private bool isAboutToBeCleared = false;

    //les controleurs de la main et des doigt
    private DetectionController.HandController handController;
    private DetectionController.FingerController fingerController;

    private Coroutine coroutine;
    private bool audioPlayed = false;

    /*****************************************************
    * AWAKE
    *
    * INFO:    Instance de la classe et position initiale.
    *
    *****************************************************/
    private void Awake()
    {
        //Initialise l'instance de cette classe
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        //Liste des objets sélectionnés par raycast
        selectedObjectsList = new List<SelectedObject>();
        selectedObjectsIDList = new List<int>();
        raycastColor = new Color32(255, 0, 0, 255);
    }

    /*****************************************************
    * START INDEX ONLY
    *
    * INFO:    Fonction appelée lorsque seulement l'index
    *          a été détecté.
    *
    *****************************************************/
    public void StartIndexOnly()
    {
        isOnlyIndexOpen = true;
        wasOnlyIndexOpen = true;
    }

    /*****************************************************
    * START INDEX AND THUMB ONLY
    *
    * INFO:    Fonction appelée lorsque seulement l'index
    *          et le pouce sont sont détectés.
    *
    *****************************************************/
    public void StartIndexThumbOnly()
    {
        isOnlyIndexThumbOpen = true;
        wasOnlyIndexThumbOpen = true;
    }

    /*****************************************************
    * STOP INDEX ONLY
    *
    * INFO:    Fonction appelée lorsqu'après avoir détecté
    *          le pouce et l'index, seulement l'index est
    *          détecté.
    *
    *****************************************************/
    public void StopIndexOnly()
    {
        isOnlyIndexOpen = false;
    }

    /*****************************************************
    * STOP INDEX AND THUMB ONLY
    *
    * INFO:    Fonction appelée lorsqu'après avoir détecté
    *          seulement l'index, détecte l'index seulement.
    *
    *****************************************************/
    public void StopIndexThumbOnly()
    {
        isOnlyIndexThumbOpen = false;
    }

    /*****************************************************
    * RESET ALL STATE
    *
    * INFO:    Lorsque l'index et le pouce ne sont pas
    *          ouverts ou qu'ils ne sont pas détectés, 
    *          on réinitialise l'état des deux doigts.
    *
    *****************************************************/
    private void ResetAllState()
    {
        timer = 0f;
        isOnlyIndexOpen = false;
        wasOnlyIndexOpen = false;
        isOnlyIndexThumbOpen = false;
        wasOnlyIndexThumbOpen = false;
    }

    /*****************************************************
    * INITIATE RAYCAST
    *
    * INFO:    Demarre le raycast effectué à partir du bout
    *          d'un doigt pour une main quelconque.
    *          
    *          Initialise le laser et la sélection d'objet
    *          par raycast et Gun gesture.
    *          
    *          Fonction appelé en boucle par le DetectionController.
    *
    *****************************************************/
    public void InitiateRaycast()
    {
        //Si la main du raycast est détecté
        if (DetectionController.GetInstance().IsHandDetected(raycastHand))
        {
            //Si le doigt du raycast est le seul d'ouvert et le Palm UI n'est pas affiché
            if (DetectionController.GetInstance().GetHand(raycastHand).IsOnlyThisFingerOpened(raycastFinger) &&
                !PalmUIController.GetInstance().IsPalmUIOpen())
            {
                //Les controleurs de la main et doigt
                 handController = DetectionController.GetInstance().GetHand(raycastHand);
                 fingerController = handController.GetFinger(raycastFinger);

                //Trace la ligne rouge dans le sens que pointe le doigt de la main 3D
               Debug.DrawRay(
                    fingerController.GetFingertipPosition(),
                    fingerController.GetFingerRay().direction,
                    raycastColor,
                    Time.deltaTime, true);

                //Permet la selection d'objets 3D avec le raycast
                SelectObjectWithRaycast(fingerController.GetFingerRay());
            }
            // Si le raycast est désactivé, on reset une fois le highlight des objets
            else { if (!isObjectsMaterialCleared) { ClearTargetedObjects(); } }
        }
        // Si la main du raycast n'est plus détecté, on reset une fois le highlight des objets
        else { if (!isObjectsMaterialCleared) { ClearTargetedObjects(); } }
    }

    /*****************************************************
    * SELECT OBJECT WITH RAYCAST (AND GUN GESTURE)
    *
    * INFO:    Change le matériel d'un objet 3D lorsque le
    *          raycast entre en contact avec lui. Si l'action
    *          shoot est effectué sur un objet, on change le
    *          material pour un autre. 
    *
    *****************************************************/
    public void SelectObjectWithRaycast(Ray _ray)
    {
        RaycastHit hit;
        raycastDistance = 0f;
        isObjectsMaterialCleared = false;

        //Maintient le matérial des objets sélectionnés, sinon remet le materiel par defaut
        UpdateSelectionMaterial(targetedObject);

        isStillPointing = false;

        //Si le le raycast entre en contact avec un objet 3D
        if (Physics.Raycast(_ray, out hit))
        {
            //Recupere l'objet pointé par le raycast et la distance qui les sépare
            Transform selection = hit.transform;
            raycastDistance = hit.distance;

            //Si l'objet pointé a le bon Tag
            if (selection.CompareTag(selectableTag))
            {
                //Recupere l'ID de l'objet pointé par le raycast 
                highlightObjectID = selection.GetInstanceID();
                Renderer selectionRenderer = selection.GetComponent<Renderer>();

                if (selectionRenderer != null)
                {
                    //Si l'utilisateur effectue un geste Tir (gun + trigger) sur l'objet
                    if (isGunActionDetected)
                    {
                        //Si l'objet n'est pas déjà sélectionné, on l'ajoute dans la liste
                        if (!selectedObjectsIDList.Contains(highlightObjectID))
                        {
                            SoundController.GetInstance().PlaySound(AudioE.select);
                            selectionRenderer.material = selectedMaterial;
                            selectedObjectsIDList.Add(highlightObjectID);
                            selectedObjectsList.Add(new SelectedObject(selection));   
                        }
                        else //Sinon on déselectionne simplement l'objet
                        {
                            SoundController.GetInstance().PlaySound(AudioE.unselect);
                            selectionRenderer.material = defaultMaterial;
                            selectedObjectsIDList.Remove(highlightObjectID);
                            for (int i = 0; i < selectedObjectsList.Count; i++)
                            {
                                if (selectedObjectsList[i].GetID() == highlightObjectID)
                                {
                                    //Detruit l'objet et le retire de la liste
                                    Destroy(selectedObjectsList[i]);
                                    selectedObjectsList.RemoveAt(i);
                                }
                            }
                        }
                    }
                    else //Pour un objet simplement pointé par le raycast
                    {
                        if (selectedObjectsIDList.Contains(highlightObjectID))
                        {
                            isStillPointing = true;
                            selectionRenderer.material = selectedMaterial;
                        }
                        else
                        {
                            isStillPointing = false;
                            selectionRenderer.material = highlightMaterial;
                        }
                    }
                }
                targetedObject = selection;
                if (!audioPlayed)
                {
                    SoundController.GetInstance().PlaySound(AudioE.collision);
                    audioPlayed = true;
                }

            }
        }
        else { isStillPointing = false; audioPlayed = false; }
    }

    /*****************************************************
    * UPDATE SELECTION MATERIAL
    *
    * INFO:    Si un objet n'est pas pointé par le raycast,
    *          on s'assure de remettre le matériel par défaut,
    *          sinon on maintient le matériel des objets
    *          qui ont été sélectionnés.
    *
    *****************************************************/
    private void UpdateSelectionMaterial(Transform _target)
    {
        if (_target != null)
        {
            Renderer renderer = _target.GetComponent<Renderer>();
            if (renderer != null)
            {
                //Si l'ID (objet 3D) fait partie de la liste des objets sélectionnés
                if (selectedObjectsIDList.Contains(highlightObjectID))
                {
                    //Matériel scintille si la raysacst est maintenu sur l'objet 
                    if (isStillPointing) { coroutine = StartCoroutine(BlinkMaterial(renderer)); }
                    //Maintient simplement le material
                    else { renderer.material = selectedMaterial; }
                }
                //Sinon remet le matériel par defaut
                else { renderer.material = defaultMaterial; }
            }
        }
    }

    /*****************************************************
    * BLINK MATERIAL
    *
    * INFO:    Si l'onjet est déjà sélectionné et qu'on
    *          garde le raycast pointé sur l'objet, on
    *          le fait scintiller.
    *
    *****************************************************/
    IEnumerator BlinkMaterial(Renderer _selected)
    {
        float t = 0f;
        Color startColor = selectedMaterial.GetColor(shaderPath);

        while (isStillPointing)
        {
            t += Time.deltaTime;
            Color newColor = Color.Lerp(startColor, Color.white, Mathf.PingPong(t / blinkSpeed, 1.0f));
            _selected.material.SetColor(shaderPath, newColor);
            yield return null;
        }
        _selected.material.SetColor(shaderPath, startColor);
        yield return null;
    }

    /*****************************************************
    * BLINK UNSELECT MATERIAL
    *
    * INFO:    
    *
    *****************************************************/
    IEnumerator BlinkUnselectMaterial(Renderer _selected)
    {
        float t = 0f;
        Color startColor = unselectMaterial.GetColor(shaderPath);

        while (isAboutToBeCleared)
        {
            t += Time.deltaTime;
            Color newColor = Color.Lerp(startColor, Color.white, Mathf.PingPong(t / blinkSpeed, 1.0f));
            _selected.material.SetColor(shaderPath, newColor);
            yield return null;
        }
        _selected.material.SetColor(shaderPath, startColor);
        yield return null;
    }

    /*****************************************************
    * CLEAR TARGETED OBJECT
    *
    * INFO:    Retire le highlight d'un objet qui ne devrait 
    *          plus l'avoir. Ex: la main sort de la zone de
    *          détection et l'objet reste pris avec le matériel.
    *
    *****************************************************/
    public void ClearTargetedObjects()
    {
        isStillPointing = false;
        if (coroutine != null) StopCoroutine(coroutine);

        //Reset une fois le highlight des objets
        if (!isObjectsMaterialCleared)
        {
            isObjectsMaterialCleared = true;
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag(selectableTag))
            {
                Renderer renderer = obj.GetComponent<Renderer>();
                if (renderer != null)
                {
                    if (renderer.material.name.Contains(highlightMaterial.name)) { renderer.material = defaultMaterial; }
                }
            }
        }
    }

    /*****************************************************
    * UNSELECT ALL OBJECT
    *
    * INFO:    Permet de tout désélectionner les objets
    *          d'un seul coup. Si le processus de délection 
    *          est maintenu en attente de façon volontaire,
    *          le material devient rouge pour indiquer qu'on
    *          est pret a tout désélectionner.
    *
    *****************************************************/
    private void UnselectAllObjects(bool _clear)
    {
        //Pour deselectionner tous les objets
        if (_clear)
        {
            for (int i = 0; i < selectedObjectsList.Count; i++)
            {
                //if(coroutine != null) StopCoroutine(coroutine);
                Renderer renderer = selectedObjectsList[i].TransformObject.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    //Remet le materiel par defaut des objets
                    isAboutToBeCleared = false;
                    renderer.material = defaultMaterial;

                    //Vide les liste de selection d'objets
                    selectedObjectsIDList.Clear();
                    selectedObjectsList.Clear();
                }
            }
            SoundController.GetInstance().PlaySound(AudioE.unselect);
        }
        else  //Material rouge pour indiquer qu'on est pret a deselectionner
        {
            for (int i = 0; i < selectedObjectsList.Count; i++)
            {
                Renderer renderer = selectedObjectsList[i].TransformObject.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    isAboutToBeCleared = true;
                    //isStillPointing = true;
                    renderer.material = unselectMaterial;
                    coroutine = StartCoroutine(BlinkUnselectMaterial(renderer));
                }
            }
        }
    }

    /*****************************************************
    * UPDATE - TARGET OBJECT
    *
    * INFO:    Detection de l'action qui consiste a effectuer
    *          un tir (trigger) avec le pouce lorsque la main
    *          fait un geste de fusil (pouce et index).
    *
    *****************************************************/
    void Update()
    {
        isGunActionDetected = false;

        //Si le Palm UI n'est pas ouvert et la main droite détecté
        if (!PalmUIController.GetInstance().IsPalmUIOpen() &&
            DetectionController.GetInstance().IsHandDetected(raycastHand))
        {
            //Étape 1: l'index et le pouce (fusil) sont ouverts
            if (wasOnlyIndexThumbOpen)
            {
                //Étape 2: le pouce est désormais fermé (trigger) et l'index est ouvert
                if (isOnlyIndexOpen && !isOnlyIndexThumbOpen)
                {
                    //Le temps du trugger (pouce: ouvert --> fermé --> ouvert)
                    timer += Time.deltaTime;
                }
                //Étape 3: l'index et le pouce sont ouverts à nouveau et le trigger respecte le delais
                if (timer < triggerTimer && timer > 0f && wasOnlyIndexOpen && isOnlyIndexThumbOpen)
                {
                    isGunActionDetected = true;
                    timer = 0f;
                }
                //Pour tout déselectionner les objets, on maintient le pouce descendu plus longtemps
                if (timer > timeBeforeUnselect && timer > triggerTimer && isOnlyIndexOpen && !isOnlyIndexThumbOpen)
                {
                    //Change le materiel de couleur (rouge)
                    UnselectAllObjects(false);
                }
                //Si apres prolongation du pouce fermé et qu'on le relève, désélectionne tous les objets.
                if (timer > timeBeforeUnselect && timer > triggerTimer && wasOnlyIndexOpen && isOnlyIndexThumbOpen)
                {
                    //Remet le materiel par defaut et vide les liste de sélection
                    UnselectAllObjects(true);
                    timer = 0f;
                }

                //timer = 0f;
            }
            //Si on ne pointe pas et que le geste Gun (pouce et index) n'est pas détecté
            if (!isOnlyIndexOpen && !isOnlyIndexThumbOpen) { ResetAllState(); }
        }
        else { ResetAllState(); }
    }

    /*****************************************************
    * GET SELECTED OBJECT
    *
    * INFO:    Retourne la liste des objets sélectionnés.
    *
    *****************************************************/
    public List<SelectedObject> GetSelectedObjects()
    {
        return selectedObjectsList;
    }

    /*****************************************************
    * GET SHAND
    *
    * INFO:    Retourne la main choisi pour effectuer la
    *          selection d'objets 3D.
    *
    *****************************************************/
    public HandsE GetHand()
    {
        return raycastHand;
    }

    /*****************************************************
    * GET FINGER
    *
    * INFO:    Retourne le doigt choisi pour effectuer la
    *          selection d'objets 3D.
    *
    *****************************************************/
    public FingersE GetFinger()
    {
        return raycastFinger;
    }

    /*****************************************************
     * GET RAYCAST DISTANCE
     *
     * INFO:    Retourne la distance entre le bout du doigt
     *          qui a le raycast et l'objet 3D pointé.
     * 
     *****************************************************/
    public float GetRaycastDistance()
    {
        return raycastDistance;
    }

    /*****************************************************
    * GET CLASS INSTANCE
    *
    * INFO:    Retourne l'instance de cette classe.
    *
    *****************************************************/
    public static SelectionController GetInstance()
    {
        return instance;
    }
}
