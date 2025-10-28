using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NotebookPhonesController : MonoBehaviour
{
    [SerializeField] GameObject PhoneNumberPrefab;
    [SerializeField] Transform WordContainer;
    [SerializeField] WordData CabinWord;
    List<GameObject> WordsInstances = new List<GameObject>();
    List<int> removedIndex = new List<int>(); // Lista para almacenar los índices eliminados
    List<WordData> InctiveWordsOnBoard = new List<WordData>();
    [SerializeField] NotebookProcessManager proccessManager;
    int i = 0;
    bool once = false;
    bool IsPhoneSlideOut;
    bool isStarting = true;
    Vector3 initialConteinerPos;

    private void Start(){
        initialConteinerPos = WordContainer.localPosition;
        Invoke("SetisStartingFalse", 2f);
        WordContainer.transform.localPosition = new Vector3(0, -300, 0);
    }

    void SetisStartingFalse()
    {
        isStarting = false;
        InctiveWordsOnBoard = new List<WordData>(WordSelectedInNotebook.Notebook.GetWordsInBeggining());
    }

    // Refresh When is added a new Phone
    public void RefreshPhones(Component component, object obj)
    {
        WordData LastPhoneAdded = (WordData)obj;

        int auxIndex = i;

        bool replaceBool = WordReplaceOther(LastPhoneAdded);

        

        // Verifica si hay un índice libre para reutilizar
        if (removedIndex.Count != 0 && !once)
        {
            auxIndex = removedIndex[0];
            removedIndex.RemoveAt(0);
        }
        else if (removedIndex.Count == 0 && !replaceBool)
        {
            i++;
        }

        if (replaceBool) return;

       /*if (LastPhoneAdded.GetIsPhoneNumberFound() && LastPhoneAdded.GetIsAPhoneNumber()) comentado porque ahora encontrár un numero automáticamente agrega su palabra.
        {
            // Entra si la palabra está agregada pero le falta el número
            foreach (GameObject phone in WordsInstances)
            {
                PhoneRowNotebookController PhoneScript = phone.GetComponent<PhoneRowNotebookController>();
                if (PhoneScript.GetWord().GetPhoneNumber() == LastPhoneAdded.GetPhoneNumber())
                {
                    PhoneScript.UpdateNumber();
                    return;
                }
            }
        }*/

        //Entra si hay un número agregado y falta su palabra
        if (SearchForAnExistingPhoneNum(LastPhoneAdded)) return;

        if (!LastPhoneAdded.GetIsAPhoneNumber()) return;

        GameObject wordaux = Instantiate(PhoneNumberPrefab, WordContainer);
        wordaux.GetComponent<PhoneRowNotebookController>().GetWordButton().onClick.AddListener(ClearUnderLine);
        wordaux.GetComponent<PhoneRowNotebookController>().GetNumButton().onClick.AddListener(ClearUnderLine);
        wordaux.GetComponent<PhoneRowNotebookController>().Initialization(LastPhoneAdded, this, proccessManager);
        WordsInstances.Add(wordaux);

        once = false;
    }

    // Función para borrar una instancia de teléfono
    public void RemovePhoneInstance(Component sender, object obj)
    {
        WordData phoneToRemove = (WordData)obj;
        List<GameObject> PhonesToRemove = new List<GameObject>();

        once = true;

        foreach (GameObject instancePhone in WordsInstances)
        {
            PhoneRowNotebookController script = instancePhone.GetComponent<PhoneRowNotebookController>();
            if (script.GetWord() == phoneToRemove)
            {
               // script.EraseAnim();
                PhonesToRemove.Add(instancePhone);

                int index = WordsInstances.FindIndex(phone => phone == instancePhone);
                if (index != -1)
                {
                    removedIndex.Add(index);
                }
            }
        }

        StartCoroutine(DeletePhone(PhonesToRemove));
    }

    // Coroutine para eliminar los números de teléfono después de la animación
    IEnumerator DeletePhone(List<GameObject> list)
    {
        yield return new WaitForSeconds(0.5f);
        foreach (GameObject phone in list)
        {
            WordsInstances.Remove(phone);
            Destroy(phone);
        }
    }

    public void DeleteAllWords(Component sender, object obj)
    {
        List<GameObject> auxList = new List<GameObject>(WordsInstances);

        StartCoroutine(DeletePhone(auxList));
        i = 0;
    }


    // Función para reemplazar un número por otro
    bool WordReplaceOther(WordData newword)
    {
        bool aux = false;
        foreach (GameObject w in WordsInstances)
        {
            if (!newword.GetWordThatReplaces()) continue;
            PhoneRowNotebookController script = w.GetComponent<PhoneRowNotebookController>();
            if (SearchForWordThatReplaceRetroactive(script.GetWord(),newword))
            {
                script.TryUpdateWord(newword);
                ClearUnderLine();
                aux = true;
            }

        }

        return aux;
    }


    public void ReplaceAllWithCabin(Component sender, object obj)
    {
        foreach (GameObject w in WordsInstances)
        {
            PhoneRowNotebookController script = w.GetComponent<PhoneRowNotebookController>();

           // script.ReplaceWordInstantly(CabinWord);
        }
    }

    bool SearchForWordThatReplaceRetroactive(WordData oldWord, WordData newWord)
    {
        WordData currentWord = newWord.GetWordThatReplaces();
        WordData startWord = oldWord;

        while (currentWord != null)
        {
            if (currentWord == startWord)
                return true;

            currentWord.SetIsFound();
            currentWord = currentWord.GetWordThatReplaces();
        }

        return false;
    }

    bool SearchForAnExistingPhoneNum(WordData word)
    {
        if (word.GetIsAPhoneNumber()) return false;

        foreach (GameObject w in WordsInstances)
        {
            PhoneRowNotebookController script = w.GetComponent<PhoneRowNotebookController>();

            if (script.GetWord() == word)
            {
                script.TryUpdateWord(word);
                word.SetIsPhoneNumberFound();
                return false;
            }

        }

        return true;
    }


    public void ClearUnderLine()
    {
        foreach (GameObject word in WordsInstances)
        {
            word.GetComponent<PhoneRowNotebookController>().ClearUnderline();
        }
    }

    void DeleteWords()
    {
        for (int i = 0; i < WordContainer.childCount; i++)
        {
            Destroy(WordContainer.GetChild(i).gameObject);
        }
    }

    public void PutingWordOnBoard(Component sender, object obj)
    {
        //if (!IsPhoneSlideOut) return;
        InctiveWordsOnBoard.Add((WordData)obj);

        InactiveWordInBoard((WordData)obj);

    }

    ViewStates actualView;
    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;
        List<WordData> Empylist = new List<WordData>();
        List<WordData> listAllWord = new List<WordData>();
        ClearUnderLine();
        foreach (GameObject instance in WordsInstances)
        {
            listAllWord.Add(instance.GetComponent<PhoneRowNotebookController>().GetWord());
        }

        if (actualView == ViewStates.BoardView)
        {
            DisableWordsOfList(InctiveWordsOnBoard, "Board", true, true);
        }
        else if (actualView == ViewStates.TVView)
        {
            DisableWordsOfList(listAllWord);
        }
        else if (actualView == ViewStates.OnTakeSomeInBoard)
        {
            DisableWordsOfList(InctiveWordsOnBoard, "Board", true, true);
        }
        else
        {
            
            DisableWordsOfList(Empylist);
        }
    }

    public void EnableInSlidePhones(Component sender, object obj)
    {
        IsPhoneSlideOut = true;

        WordContainer.transform.localPosition = initialConteinerPos;
    }

    public void DisableInSlidePhones(Component sender, object obj)
    {
        IsPhoneSlideOut = false;
        WordContainer.transform.localPosition = new Vector3(0,-300,0);
    }

    
    void DisableWordsOfList(List<WordData> list, string material = "", bool changes = true, bool thickness = false)
    {
        foreach (GameObject instanceBTN in WordsInstances)
        {
            bool isActive = true;
            PhoneRowNotebookController Wordinstance = instanceBTN.GetComponent<PhoneRowNotebookController>();
            Wordinstance.TryActiveWord(true);
            Wordinstance.SetInactive(false);

            foreach (WordData word in list)
            {
                if (Wordinstance.GetWord() == word)
                {
                    isActive = false;
                    Wordinstance.SetInactive(true);
                    Wordinstance.TryActiveWord(false);
                }
            }
            if (isActive)
            {
                //palabra que sigue activa
                Wordinstance.ApplyMaterial(material);
                if (changes) Wordinstance.ApplyThicknessAnim(thickness);
            }
        }
    }

    void InactiveWordInBoard(WordData word)
    {
        foreach(GameObject instanceBTN in WordsInstances)
        {
            PhoneRowNotebookController script = instanceBTN.GetComponent<PhoneRowNotebookController>();

            if (script == null) return;

            if(script.GetWord() == word)
            {
                script.InactiveDirectly();
            }
        }
    }


    public float waitSlidePhoneUp = 0;
    private Queue<IEnumerator> actionQueue = new Queue<IEnumerator>();
    private bool isRunning = false;

    public void AddAction(IEnumerator action)
    {
        actionQueue.Enqueue(action);
        if (!isRunning)
            StartCoroutine(ProcessQueue());
    }

    private IEnumerator ProcessQueue()
    {
        isRunning = true;

        yield return new WaitForSeconds(waitSlidePhoneUp);
        waitSlidePhoneUp = 0;
        while (actionQueue.Count > 0)
        {
            yield return StartCoroutine(actionQueue.Dequeue());
        }
        isRunning = false;
    }

}
