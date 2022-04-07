using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class PlayerRTSController : MonoBehaviour
{
 
    public static PlayerRTSController instance;

    bool unitexist = false;
    Vector3 startWorldMousePosition;
    Vector3 startRawMousePosition;     
    Vector3 lastmouseposition;      // Delete later.
    [SerializeField] private CameraController camController;
    [SerializeField] private Camera cam;
    [SerializeField] private RectTransform selectionarea;
    [SerializeField] private float distancebetweenUnits = 1.25f;
    Vector3 tempcenter,tempsize;
    List<SelectableObject> selectedObjects = new List<SelectableObject>();
    List<RTSUnit> selectedPlayerUnits = new List<RTSUnit>();
    RaycastHit[] rayhitResults = new RaycastHit[10];        //Sağ tık aksiyon seçimleri için atılan ray için infolar.

    int terrainlayermask = 1 << 8;
    int interactionLayer =0;


    private bool buildPlacementOn = false;
    private bool wallPlacementOn = false;
    private bool wallPlacementFirstPhaseOn = false;
    private bool wallPlacementSecondPhaseOn = false;
    private bool SelectionOn = false;
    private bool mouseOverUI = false;
    private Vector3 currentWallStartPoint;

    public List<SelectableObject> SelectedObjects {get{return selectedObjects;}}
    public List<RTSUnit> SelectedPlayerUnits { get {return selectedPlayerUnits;}}

    //Seçilen ünitelerin genel türü.
    private RTSUnit.UnitTypes currentunittype;  

    //Single build place ref for the build placement.
    private BuildPlaceObject currentBuildPlacement;

    //Wall construction refs.
    private BuildPlaceObject edgeWall;
    private BuildPlaceObject mainWall;
    private AbilityRequirements[] edgeWallRequirement;
    private AbilityRequirements[] mainWallRequirement;
    private float edgeDistance;
    private float mainWallDistance;
    private List<BuildPlaceObject> currentPlacedWalls = new List<BuildPlaceObject>();



    private void Awake() 
    {
        instance = this;
    }

    private void Start() 
    {
        selectionarea.gameObject.SetActive(false);
 
        //Bit operations for unit interaction layer.
        int unitInteraction = 1 << LayerMask.NameToLayer("PlayerUnitInteractionLayer");
        interactionLayer = unitInteraction | terrainlayermask;
    }

    private void Update() 
    {
        if(GameplayUIManager.instance.menuOpen) return;

        //Kamera kontrolleri
        if(Input.GetKey(KeyCode.A))
        {
            //Kamera sola doğru gidecek.
            camController.MoveCamera(Vector3.left);
        }

        if(Input.GetKey(KeyCode.D))
        {
            //Kamera sağa doğru gidecek.
            camController.MoveCamera(Vector3.right);
        }

        if(Input.GetKey(KeyCode.W))
        {
            //Kamera yukarı doğru gidecek.
            camController.MoveCamera(Vector3.forward);
        }

        if(Input.GetKey(KeyCode.S))
        {
            //Kamera sola doğru gidecek.
            camController.MoveCamera(Vector3.back);
        }

        if(Input.GetKey(KeyCode.Escape))
        {
            GameplayUIManager.instance.OpenMainMenuPanel();
        }

        //Kamera yakınlaşma olayı (FOV).
        if(Input.mouseScrollDelta.y == -1 || Input.mouseScrollDelta.y == 1)
        {
            camController.ChangeCamFov(Input.mouseScrollDelta.y);
        }

        // Bina Kurulumu var iken çalışan fonksiyon.
        if(buildPlacementOn)
        {
            //Verilen bina mouse pozisyonunu takip edecek.       
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit,100,terrainlayermask))    //Planede ki yer esas alınacak.
            {
                if(hit.collider.gameObject.CompareTag("Terrain"))
                 currentBuildPlacement.transform.position = hit.point;
            }      
        }

        if(wallPlacementOn)
        {
            //First phase.
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(wallPlacementFirstPhaseOn)
            {
                if(Physics.Raycast(ray, out hit,100,terrainlayermask))    //Planede ki yer esas alınacak.
                {
                    if(hit.collider.gameObject.CompareTag("Terrain"))
                    edgeWall.transform.position = hit.point;
                } 
            }

            
            
            /*At second phase after the placing the first edge, calculating the distance between first position and recent position and
            place the edges and main wall bodies between this distance and keep them at a list.*/
            else if(wallPlacementSecondPhaseOn)
            {
                       
                if(Physics.Raycast(ray, out hit,100,terrainlayermask))
                {
                    if(hit.collider.gameObject.CompareTag("Terrain"))
                    {
                            //First determine the distance.
                        Vector3 distance = hit.point - currentWallStartPoint;
                        distance.y = 0; //Ignore y.
                        float distanceMeter = distance.magnitude;
                        
                        distanceMeter = distanceMeter - (edgeDistance/2);

                        //Then find out how many wall pieces required for distance.
                        int pieceCount = (int)(distanceMeter/(edgeDistance+mainWallDistance));

                        pieceCount *=2;

                        int leftVal = (int)(distanceMeter%(edgeDistance+mainWallDistance));

                        if(leftVal > 0) pieceCount +=2;

                        //Then check the current wall pieces create or delete when necessary.

                        int currentPieceCount = currentPlacedWalls.Count -1;    //Ignore the first piece.

                        int difference = pieceCount - currentPieceCount;

                        Vector3 direction = distance.normalized;

                        //Positive,need to create extra pieces.
                        if(difference > 0)
                        {
                            for(int i =0; i< difference;i++)
                            {
                                int leftV = (i%2);
                                
                                //Main Body wall create.
                                if(leftV == 0)
                                {
                                    BuildPlaceObject obj = Instantiate(mainWall,Vector3.zero,Quaternion.identity);
                                    currentPlacedWalls.Add(obj);
                                }
                                
                                //1,Edge create.
                                else
                                {
                                    BuildPlaceObject obj= Instantiate(edgeWall,Vector3.zero,Quaternion.identity);
                                    currentPlacedWalls.Add(obj);
                                }
                            }
                        }

                        //Negative, need to delete unnecassary pieces.
                        else if(difference < 0)
                        {
                            int abdDif = Mathf.Abs(difference);

                            for(int i =0; i < abdDif;i++)
                            {
                                BuildPlaceObject obj = currentPlacedWalls[currentPlacedWalls.Count-1];
                                currentPlacedWalls.RemoveAt(currentPlacedWalls.Count-1);
                                Destroy(obj.gameObject);
                            }

                            //After delete extra pieces adjust to positions and rotations current ones.
                            
                            
                        }

                        //Else do not create extra pieces.

                        //Always adjust positions and rotations.
                        float deltaval = (edgeDistance+mainWallDistance)/2;

                        Vector3 lastPlacedObjPos = currentPlacedWalls[0].transform.position;

                            //Adjust positions;
                            for(int i = 1; i < currentPlacedWalls.Count;i++)
                            {
                                currentPlacedWalls[i].transform.position = lastPlacedObjPos +(direction*deltaval);
                                lastPlacedObjPos = currentPlacedWalls[i].transform.position;
                            }

                            //Adjust rotations.
                            for(int i=0;i < currentPlacedWalls.Count;i++)
                            {
                                currentPlacedWalls[i].transform.right = direction;
                            }


                    }

                    
                }
            }
                   
        }

        //Mouse sol tık basıldığında.
        if(Input.GetMouseButtonDown(0))
        {
            mouseOverUI = false;
            if(buildPlacementOn || wallPlacementOn) return;  

            //İlk tıklamada mouseın ui üzerinde olup olmadığının kontrol edilmesi, normal oyun için işlemler yok sayılıyor bu durumda.
            if (EventSystem.current.IsPointerOverGameObject())
            {
                mouseOverUI = true;
                return;
            }

            //Seçilmiş objelerin selectionlarını kaldır.
            for(int i = 0; i < selectedObjects.Count;i++)
            {
                if(selectedObjects[i] != null)
                {
                    selectedObjects[i].ObjectDeselected();
                }
            }

            //Seçilmiş üniteleri ve objeleri temizle.
            selectedObjects.Clear();
            selectedPlayerUnits.Clear();
            MouseCursorManager.instance.SetCursor(MouseCursorManager.MouseCursors.Neutral);

            //Plane için ray gönderilip, tıklandığı anda ilk mouse pozisyonunun bulunması.
            SelectionOn = true;
  
                Ray ray2;
                startRawMousePosition = Input.mousePosition;
                selectionarea.gameObject.SetActive(true);
                ray2 = cam.ScreenPointToRay(startRawMousePosition);           
                RaycastHit hit2;

                if(Physics.Raycast(ray2, out hit2,100,terrainlayermask))   //Sadece plane için ayarla.
                {
                    if(hit2.transform.gameObject.CompareTag("Terrain"))
                    {
                        startWorldMousePosition = hit2.point;
                    }

                }      
        }

        //Mouse Sol tık basılıp bırakılmadığı, objelerin seçimini için seçim görselinin güncellenmesi
        if(Input.GetMouseButton(0))
        {
            if(buildPlacementOn || wallPlacementOn) return;

            if(SelectionOn)
            {
                 //Burası sadece selection area yeri ve size ayarlanması için.
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 selectionPos = ((currentMousePosition - startRawMousePosition)/2) + startRawMousePosition;
                selectionarea.position = selectionPos;
                Vector3 scaleSize = currentMousePosition - startRawMousePosition;
                selectionarea.localScale = scaleSize;
            }

            
        }

        //Sol tık bastıktan sonra kaldırıldığında çalışan kısım.
        if(Input.GetMouseButtonUp(0))
        {

            //Başta mouse ui üzerine tıklanmışsa yine aynı şekilde mouse için normal oyun işlemleri yapılmayacak.
            if (mouseOverUI)
            {
                return;
            }

            //Eğer bina yerleşimi yapılıyorsa, bina yerleşimi için çalışacak bu tıklama.
            if(buildPlacementOn)
            {
                //İlk önce available durumda mı var olan placement kontrol edilecek öyle ise placement yapılacak.
                if(currentBuildPlacement.PlacementAvailable)    EndBuildPlacement(true);  
                else    //Değil ise error yazısı çıkabilir şuanlık boş.
                {
                    Debug.Log("Yerleştirilme uygun değil");
                }

                return;
            }

            if(wallPlacementOn)
            {

                //Check the all placements is availabled at first.

                    for(int i=0; i < currentPlacedWalls.Count;i++)
                    {
                        if(!currentPlacedWalls[i].PlacementAvailable)
                        {
                            Debug.Log("Yerleştirilme uygun değil");
                            return;
                        }
                    }

                    if(wallPlacementFirstPhaseOn)
                    {
                        wallPlacementFirstPhaseOn = false;
                        wallPlacementSecondPhaseOn = true;
                        EndWallPlacementFirstPhase(true);
                    }

                    else if(wallPlacementSecondPhaseOn)
                    {
                        EndWallPlacementSecondPhase(true);
                    }
                return;
            }

            //Mouse tuşuna basım kaldırıldığında ve buildplacement aktif değil ise abilities ui çalışacak.
            GameplayUIManager.instance.CloseAbilitiesUI();
            GameplayUIManager.instance.CloseUIPanel();

            //Bu kısım sol kaldırıldığında arada kalan objelerin seçilmesi için yazılan kısım.
                // Anlık mouse pozisyonu alınıyor.
                Vector3 lastPosition = Vector3.zero;
                Ray ray;
                ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                //Mouse son pozisyonu ray gönderiliyor.
                if(Physics.Raycast(ray, out hit,100,terrainlayermask))
                {
                    if(hit.transform.gameObject.CompareTag("Terrain"))
                    {
                        lastmouseposition = hit.point;
                    }
                }

                 SelectableObjectTypes currentslectabletype = SelectableObjectTypes.Default;
                // Seçilebilen objelerin saptanması için başlangıç ve bitiş mouse pozisyonlarına göre box collider oluşturuluyor.
                Vector3 differencePos = lastmouseposition - startWorldMousePosition;
                //There will be 2 option, if there is small area or just one click we will send just a ray or we will create create a box.
                //Option 1 just send a single ray and select one obj.
                if(differencePos.sqrMagnitude < 1)
                {
                    ray = cam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit[]  hits = Physics.RaycastAll(ray,100);
                    for(int i = 0; i < hits.Length;i++)
                    {
                        if(hits[i].transform.TryGetComponent<SelectableObject>(out SelectableObject selectedObject))
                        {
                            selectedObjects.Add(selectedObject);
                            currentslectabletype = selectedObject.GetSelectionType();
                            break;
                        }
                    }
                }

                //Option 2 create a big selection area and select all inside of this area.
                else
                {
                    Vector3 halfsize = new Vector3(Mathf.Abs(differencePos.x/2),10,Mathf.Abs(differencePos.z/2));
                    Vector3 center = startWorldMousePosition + differencePos/2;
                    Collider[] allhits;
                    tempcenter = center;
                    tempsize = halfsize *2;
                    allhits = Physics.OverlapBox(center,halfsize);  //Box colliderın aktif edilmesi.

                    //Burada ilk ünitenin tipi belirlendikten sonra, karışık seçim olmaması için kalan ünitelerde ona göre belirlenecek.
                    bool typeSelected = false;
                    for(int i = 0; i  < allhits.Length; i++)
                    {   
                        if(allhits[i].TryGetComponent<SelectableObject>(out SelectableObject selectedObject))
                        {
                            //İlk objenin türünün alınması için.
                            if(!typeSelected)
                            {
                                currentslectabletype = selectedObject.GetSelectionType();
                                selectedObjects.Add(selectedObject);
                                typeSelected = true;
                                continue;
                            }

                            //Var olan nesnenin öncelik sırası daha öndeyse(rakam olarak küçükse) tür ona dönüyor.
                            if(ObjectSelectionManager.instance.GetPriorityOfObject(selectedObject.GetSelectionType()) < ObjectSelectionManager.instance.GetPriorityOfObject(currentslectabletype))
                            {
                                currentslectabletype = selectedObject.GetSelectionType();
                                //Önceki başka türlerden seçilenler listeden çıkarılması lazım.
                                selectedObjects.Clear();
                                selectedObjects.Add(selectedObject);
                            }
                            
                            //Eğer var olan türden devam ediliyorsa sadece var olan objeyi listeye ekliyoruz.
                            else if(selectedObject.GetSelectionType() == currentslectabletype)
                            {
                                selectedObjects.Add(selectedObject);
                            }
                        }
                    }
                }

                //Bu kısımda seçilen nesneleri select ediyoruz
                if(selectedObjects.Count > 0)   //Obje seçildiğinden emin olmak için.
                {
                    //Eğer seçilen obje tipi için multiple selection aktif değil ise ilk seçilen objeyi seçilebilir hale getirip bırakıyoruz
                    if(!ObjectSelectionManager.instance.IsMultipleSelectionOn(currentslectabletype))
                    {
                        selectedObjects[0].ObjectSelected(true);
                    }

                    //Multiple selection seçilen obje türü için geçerli olduğu durum.
                    else
                    {
                        // Unitler için özel durum, ayrı bir listede tutulmaları gerektiği için, ayırıyoruz.
                        if(currentslectabletype == SelectableObjectTypes.playerUnits)
                        {
                            //Only one unit selected.
                            if(selectedObjects.Count == 1)
                            {
                                selectedObjects[0].ObjectSelected(true);
                                selectedPlayerUnits.Add(selectedObjects[0].GetComponent<RTSUnit>());
                            }

                            //Multiple unit selected.
                            else
                            {
                                for(int i = 0 ; i < selectedObjects.Count; i++)
                                {   
                                  selectedObjects[i].ObjectSelected(false); 
                                  //Since they'r player units we adds them to the list.
                                  selectedPlayerUnits.Add(selectedObjects[i].GetComponent<RTSUnit>());
                                }
                            }  
                        }

                        //Other selectable objects.
                        else
                        {
                            if(selectedObjects.Count == 1)                  
                                selectedObjects[0].ObjectSelected(true);
                            else
                            {
                                for(int i = 0 ; i < selectedObjects.Count; i++)
                                    selectedObjects[i].ObjectSelected(false); 
                            }
                        }
                    }  
                }        
                selectionarea.gameObject.SetActive(false);
        }

        //Mouse sağ tık fonksiyonları.
        if(Input.GetMouseButtonDown(1)) 
        {
            if(buildPlacementOn)
            {
                //Sağ tık basılırsa iptal edilecek placement olayı.
                EndBuildPlacement(false);
                return;
            }

            if(wallPlacementOn)
            {
                
                if(wallPlacementFirstPhaseOn)
                    EndWallPlacementFirstPhase(false);
                
                else if(wallPlacementSecondPhaseOn)
                    EndWallPlacementSecondPhase(false);

                return;
            }

                     
                //Seçili ünite varsa ünite hamleleri için.
                if(selectedPlayerUnits.Count > 0)
                {
                
                    //Ray gönderip tıklanılan yerin pozisyonu alınacak.
                   Ray mouseRay  = cam.ScreenPointToRay(Input.mousePosition);
                   int hitsize =  Physics.RaycastNonAlloc(mouseRay,rayhitResults,Mathf.Infinity,interactionLayer);        //Genel ray gönderdik.

                    int movePosIndex = 0;
                    CalculateRowandColumnValues(out int columnvalue,selectedPlayerUnits.Count);

                    for(int i= 0; i < selectedPlayerUnits.Count;i++) 
                    {
                        bool interactedWithObject  = false;

                        if(selectedPlayerUnits[i] == null) continue;

                        //İlk olarak interactable object bulunup ve kontrol edilip onla etkileşime giriyor mu kontrol ediliyor.
                        for(int j = 0; j < hitsize ; j++)
                        {
                            if(rayhitResults[j].transform.TryGetComponent<ObjectPlayerInteraction>(out ObjectPlayerInteraction obj))
                            {
                               // Debug.Log("Interactable object");
 
                                if(obj.StartObjectInteraction(selectedPlayerUnits[i])) 
                                {
                                    interactedWithObject = true;
                                    break;
                                     //Bu ünite için eylem gerçekleştiğinden döngüden çıkılıyor.
                                }
                            }            
                        }
                        

                        //Etkileşime girilmemiş ise ünite hareketi için terrain bilgisi alınıp hareket başlatılıyor.
                        if(!interactedWithObject)
                        {
                                for(int j = 0 ; j < hitsize; j++)
                                {
                                    if(rayhitResults[j].transform.CompareTag("Terrain"))
                                    {   
                                        selectedPlayerUnits[i].MoveToPosition(CalculateUnitPosition(rayhitResults[j].point,columnvalue,movePosIndex));
                                        movePosIndex++;
                                        break;
                                    }
                                }
                        }
                    }
                    
                    #region old part
                    // //Temp kısım.
                    // currentunittype = RTSUnit.UnitTypes.Worker; 

                    // //Worker eylemleri için olan kısım.
                    // if(currentunittype == RTSUnit.UnitTypes.Worker)
                    // {
                    //     //Workera öncelikle hit yaptığımız şeylere bakarak workerın aksiyonu belirlenecek.

                    //     //Base action move olarak belirlenecek.
                    //     UnitActionStateManager.ActionStates selectedAction = UnitActionStateManager.ActionStates.move;
                    //     int hitindex = 0;

                        
                    //     for(int i = 0; i < hitsize;i++)
                    //     {
                    //         Debug.Log(rayhitResults[i].transform.name);
                    //     }



                    //     for(int i = 0; i < hitsize;i++)
                    //     {
                            

                    //         if(rayhitResults[i].transform.CompareTag("Resources"))
                    //         {
                    //             selectedAction = UnitActionStateManager.ActionStates.gatherResources;
                    //             hitindex = i;
                    //             break;
                    //         }

                    //         if(rayhitResults[i].transform.CompareTag("Builds"))
                    //         {
                    //             //Bu noktada buildin tamire ve yapılmasına ihtiyacı var mı bakılıp ona göre seçim yapılabilir.
                    //             selectedAction = UnitActionStateManager.ActionStates.buildConstruction;
                    //             hitindex = i;
                    //             break;
                    //         }
                            
                    //         else if(rayhitResults[i].transform.CompareTag("Plane"))
                    //         {
                    //             selectedAction = UnitActionStateManager.ActionStates.move;
                    //             hitindex = i;
                                
                    //         }
                    //     }

                    //     //Seçilen workerlar için sadece movement eylemi seçildi ise.
                    //     if(selectedAction == UnitActionStateManager.ActionStates.move)
                    //     {
                    //         int maxColumn;
                    //         CalculateRowandColumnValues(out maxColumn,selectedunits.Count);

                    //             for(int i= 0; i < selectedunits.Count;i++)
                    //             {
                    //                 //Seçilen üniteler o pozisyona move ettirilecek.
                    //                 selectedunits[i].MoveToPosition(CalculateUnitPosition(rayhitResults[hitindex].point,maxColumn,i));
                    //             }
                    //     }

                    //     //Seçilen workerlar için resource gathering eylemi seçildiği durum.
                    //     if(selectedAction == UnitActionStateManager.ActionStates.gatherResources)
                    //     {
                    //         for(int i = 0; i  < selectedunits.Count; i++)
                    //         {
                    //             selectedunits[i].GetComponent<WorkerUnit>().GatherResource(rayhitResults[hitindex].transform.GetComponent<Resources>());
                    //         }
                    //     }

                    //     if(selectedAction == UnitActionStateManager.ActionStates.buildConstruction)
                    //     {
                    //         Debug.Log("Build construct action");

                    //         for(int i = 0; i  < selectedunits.Count; i++)
                    //         {
                    //             selectedunits[i].GetComponent<WorkerUnit>().StartBuildConstruction(rayhitResults[hitindex].transform.GetComponent<Buildings>());
                    //         }
                    //     }

                    // }   

                    #endregion         
                }  
        }

        //CURSOR changes.
            if(selectedPlayerUnits.Count > 0 && !EventSystem.current.IsPointerOverGameObject())
            {   
                bool cursorState = false;
                Ray mouseRay  = cam.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(mouseRay,out RaycastHit rayhit,Mathf.Infinity,interactionLayer))
                {
                    //Debug.Log("Check interactables cursors " + rayhit.collider.gameObject);
                    for(int i = 0; i< selectedPlayerUnits.Count;i++)
                    {
                        if(rayhit.collider.gameObject.TryGetComponent<ObjectPlayerInteraction>(out ObjectPlayerInteraction obj))
                        {
                            cursorState = obj.SendObjectCursorInformation(selectedPlayerUnits[i]);
                            if(cursorState) break;
                        }
                    }
                    if(!cursorState) MouseCursorManager.instance.SetCursor(MouseCursorManager.MouseCursors.Neutral);
                    
                }                
            }
                    //Only change it to default cursor, when it's different than default.
            else if(MouseCursorManager.instance.CurrentCursortype != MouseCursorManager.MouseCursors.Neutral)
            {
                MouseCursorManager.instance.SetCursor(MouseCursorManager.MouseCursors.Neutral);
            }         
    }

    //Dikdörtgene ulaşacak şekilde pozisyonlar bulunur.
    public Vector3 CalculateUnitPosition(Vector3 basePoint,int maxColumn,int index)
    {
        //Distance bilgisi mevcut.
            float xDistanceMultiplier = index % maxColumn;
            int zDistanceMultiplier = (int)(index / maxColumn); 

            Vector3 finalPos = new Vector3(basePoint.x + (xDistanceMultiplier * distancebetweenUnits),basePoint.y,basePoint.z - (zDistanceMultiplier * distancebetweenUnits));
            return finalPos;
   }

    //TODO duruma göre eklenebilir = Sıkışık yerlerde etrafta ki objelere dikkat edilerek yeni formasyonlar yapılabilir.
    public void CalculateRowandColumnValues(out int column, int totalUnitSize)
    {
        int row = 0;
        column = 0;

        float x = Mathf.Sqrt(totalUnitSize);
        row = (int)Mathf.Round(x);

        float y = totalUnitSize/row;
        int z = (int)y;
        int w = totalUnitSize % row;
        column = z+w;
    }

    public void RemoveRTSUnitFromList(RTSUnit unit)
    {
        selectedPlayerUnits.Remove(unit);
    }

    public void StartBuildPlacement(BuildPlaceObject buildplaceref)
    {
        AbilityUISlot.buttonsAvailable = false;
        currentBuildPlacement = buildplaceref;
        buildPlacementOn = true;
    }

    public void StartWallPlacement(BuildPlaceObject edgewall,BuildPlaceObject mainwall,AbilityRequirements[] edgeRequirement,AbilityRequirements[] mainWallRequirements,float edgedist,float maindist)
    {
        AbilityUISlot.buttonsAvailable = false;
        this.edgeWall = edgewall;
        this.mainWall = mainwall;
        edgeWallRequirement = edgeRequirement;
        mainWallRequirement = mainWallRequirements;
        edgeDistance = edgedist;
        mainWallDistance = maindist;
        wallPlacementOn = true;
        wallPlacementFirstPhaseOn = true;
        currentPlacedWalls.Add(this.edgeWall);
    }


    public void EndBuildPlacement(bool state)
    {
        //Kurulma onaylanmışsa 
        if(state)
        {  
            //We find the mouse position on terrain at this place and send it to the build placement object for placing.
            Ray ray1 = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit1;

            if(Physics.Raycast(ray1, out hit1,100,terrainlayermask))    //Only terrain.
            {
                currentBuildPlacement.PlaceBuildPrefab(hit1.point,currentBuildPlacement.transform.rotation,out GameObject build);
            }     
        }

        //if it's not succeded just cancel and destroy placement prefab.
        else
        {
            Destroy(currentBuildPlacement.gameObject);
        }

        buildPlacementOn = false;
        AbilityUISlot.buttonsAvailable = true;
    }

    public void EndWallPlacementFirstPhase(bool state)
    {
        //Just place the placeprefab and add it to the list.
        if(state)
        {
            Ray ray1 = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray1, out hit,100,terrainlayermask))    //Only terrain.
            {
                edgeWall.transform.position = hit.point;
                currentWallStartPoint = hit.point;
            }
        }

        //Cancelled.
        else
        {
            Destroy(edgeWall.gameObject);
            wallPlacementOn = false;
            wallPlacementFirstPhaseOn = false;
            wallPlacementSecondPhaseOn = false;
            AbilityUISlot.buttonsAvailable = true;
            currentPlacedWalls.Clear();
        }
    }

    public void EndWallPlacementSecondPhase(bool state)
    {
        if(state)
        {
            for(int i=0; i < currentPlacedWalls.Count;i++)
            {
                int val = i%2;
                if(val == 0) //Edges
                    currentPlacedWalls[i].Requirements = edgeWallRequirement;
                
                else //Mainwalls
                    currentPlacedWalls[i].Requirements = mainWallRequirement;
                
                currentPlacedWalls[i].PlaceBuildPrefab(currentPlacedWalls[i].transform.position,currentPlacedWalls[i].transform.rotation,out GameObject build);
                if(i == 0)
                {
                    for(int j =0; j< selectedPlayerUnits.Count;j++)
                    {
                        build.GetComponent<Buildings>().StartUnitsInteractions(selectedPlayerUnits[j]);
                    }
                }
            }

        }

        else
        {
             for(int i=0; i < currentPlacedWalls.Count;i++)
            {
                Destroy(currentPlacedWalls[i].gameObject);
            }
        }

        wallPlacementOn = false;
        wallPlacementFirstPhaseOn = false;
        wallPlacementSecondPhaseOn = false;
        AbilityUISlot.buttonsAvailable = true;
        currentPlacedWalls.Clear();


    }

    private void OnDrawGizmos() 
    {
        Gizmos.DrawCube(tempcenter,tempsize);
    }




}
