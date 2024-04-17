
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InAppSys : Singleton<InAppSys>
{


//    // the game controller to notify when the user purchase more powerballs
//    //[SerializeField] private GameController m_gameController;

//    // where to record to display the current price for the IAP item
//    [SerializeField] private Text m_priceText;

//    // purchasable IAP products we've configured on the Oculus Dashboard
//    private const string CONSUMABLE_1 = "HDVideos";

//    private bool IsPurchase = false;

//    public bool GetIsPurchase
//    {
//        get
//        {
//            return IsPurchase;
//        }
//    }

//    private Main m_Main;

//    private UnityAction[] MainCallbacks;

//    protected override void Awake()
//    {
//        base.Awake();
//        DontDestroyOnLoad(this.gameObject);
//    }

//    public void InitIAP(params UnityAction[] mainCallback)
//    {
//        MainCallbacks = mainCallback;
//        m_Main = GameObject.Find("Main").GetComponent<Main>();
//        Oculus.Platform.Core.Initialize("2503935296313353");
//        Oculus.Platform.Entitlements.IsUserEntitledToApplication().OnComplete(callbackmethd);
//    }

//    void callbackmethd(Message msg)
//    {
//        if (msg.IsError)
//        {
//            Debug.Log("************* " + msg.GetError().Message + " *************");
//            throw new UnityException(msg.GetError().Message);
//        }
//        else
//        {
//            IAP.GetViewerPurchases().OnComplete(ShowViewerPurchases);
//        }
//    }

//    // 구매 항목 검색
//    void ShowViewerPurchases(Message<PurchaseList> msg)
//    {
//        Debug.Log("************* ShowViewerPurchases *************");

//        if (msg.IsError)
//        {
//            Debug.Log("************* " + msg.GetError().Message + " *************");
//            return;
//        }
//        else
//        {
//            PurchaseList list = msg.GetPurchaseList();

//            if (list == null)
//            {
//                Debug.Log("PurchaseList is Null");
//                DataManager.Inst.SetIsPurchase = false;
//                return;
//            }

//            Debug.Log("PurchaseList.Count : " + list.Count);

//            if (list.Count != 0)
//            {
//                IsPurchase = true;
//                DataManager.Inst.SetIsPurchase = true;
//            }

//            for (int i = 0; i < list.Count; i++)
//            {
//                Debug.Log("Purchase Sku : " + list[i].Sku + " / ID : " + list[i].ID);
//            }
//        }

//        MainCallbacks[0].Invoke();
//    }

//    public void BuyHDVideo(params UnityAction[] callback)
//    {

//        MainCallbacks = callback;
//#if UNITY_EDITOR        
//        IsPurchase = true;
//        DataManager.Inst.SetIsPurchase = true;
//        MainCallbacks[0].Invoke();
//        Debug.Log("************* Puchase Complete *************");
//#else
//        IAP.LaunchCheckoutFlow(CONSUMABLE_1).OnComplete(BuyHDVideoCallback);        
//#endif
//    }

//    private void BuyHDVideoCallback(Message<Purchase> msg)
//    {
//        Debug.Log("************* BuyHDVideoCallback *************");

//        if (msg.IsError)
//        {
//            Debug.Log("************* " + msg.GetError().Message + " *************");
//        }
//        else
//        {
//            IsPurchase = true;
//            DataManager.Inst.SetIsPurchase = true;
//            MainCallbacks[0].Invoke();
//            Debug.Log("************* Puchase Complete *************");
//            //m_Main.GetHDBuntton().transform.GetComponentInChildren<Text>().text = "고화질\n3.83GB";
//            //m_priceText.text = "결제 완료";
//        }
//    }


//    // SKU에서 사용 가능한 항목 및 가격 목록 검색
//    void GetProductsBySKUCallback(Message<ProductList> msg)
//    {
//        if (msg.IsError)
//        {
//            //PlatformManager.TerminateWithError(msg);
//            Debug.Log("************* " + msg.GetError().Message + " *************");
//            return;
//        }

//        foreach (Product p in msg.GetProductList())
//        {
//            Debug.LogFormat("Product: sku:{0} name:{1} price:{2}", p.Sku, p.Name, p.FormattedPrice);
//            if (p.Sku == CONSUMABLE_1)
//            {
//                m_priceText.text = p.FormattedPrice;
//            }
//        }
//    }

//    // fetches the Durable purchased IAP items.  should return none unless you are expanding the
//    // to sample to include them.
//    public void FetchPurchasedProducts()
//    {
//        IAP.GetViewerPurchases().OnComplete(GetViewerPurchasesCallback);
//    }

//    void GetViewerPurchasesCallback(Message<PurchaseList> msg)
//    {
//        if (msg.IsError)
//        {
//            //PlatformManager.TerminateWithError(msg);
//            return;
//        }

//        foreach (Purchase p in msg.GetPurchaseList())
//        {
//            Debug.LogFormat("Purchased: sku:{0} granttime:{1} id:{2}", p.Sku, p.GrantTime, p.ID);
//        }
//    }

//    public void BuyPowerBallsPressed()
//    {
//#if UNITY_EDITOR
//        //m_gameController.AddPowerballs(1);
//#else
//        IAP.LaunchCheckoutFlow(CONSUMABLE_1).OnComplete(LaunchCheckoutFlowCallback);
//#endif
//    }

//    private void LaunchCheckoutFlowCallback(Message<Purchase> msg)
//    {
//        if (msg.IsError)
//        {
//            //PlatformManager.TerminateWithError(msg);
//            Debug.Log("************* " + msg.GetError().Message + " *************");
//            return;
//        }

//        Purchase p = msg.GetPurchase();
//        Debug.Log("purchased " + p.Sku);
//        //m_gameController.AddPowerballs(3);
//    }
}
