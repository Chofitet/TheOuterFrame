using System.Collections;
using System.Collections.Generic;
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
    int i = 0;
    bool once = false;
    bool IsPhoneSlideOut;
    bool isStarting = true;

    private void Start(){ 
        Invoke("SetisStartingFalse", 2f);
        
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

        if (LastPhoneAdded.GetIsPhoneNumberFound() && LastPhoneAdded.GetIsAPhoneNumber())
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
        }

        //Entra si hay un número agregado y falta su palabra
        if (SearchForAnExistingPhoneNum(LastPhoneAdded)) return;

        GameObject wordaux = Instantiate(PhoneNumberPrefab, WordContainer);
        wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
        wordaux.GetComponent<PhoneRowNotebookController>().Initialization(LastPhoneAdded, isStarting);
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
                script.EraseAnim();
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
                script.ReplaceNumber(newword);
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

            script.ReplaceWordInstantly(CabinWord);
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

            if (script.GetWord().GetPhoneNumber() == word.GetPhoneNumber())
            {
                if (!script.GetWord().GetIsPhoneNumberFound()) continue;

                script.ReplaceNumberWithWord(word);
                word.SetIsPhoneNumberFound();
                return true;
            }

        }

        return false;
    }


    WordData FindWordToReplaceNum(WordData Num)
    {
        List<WordData> words = WordSelectedInNotebook.Notebook.GetWordsList();
        foreach (WordData word in words)
        {
            if(Num.GetName() == word.GetPhoneNumber())
            {
                return word;
            }
        }

        return null;
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
        if (!IsPhoneSlideOut) return;
        InctiveWordsOnBoard.Add((WordData)obj);
        DisableWordsOfList(InctiveWordsOnBoard);
    }

    ViewStates actualView;
    public void CheckView(Component sender, object obj)
    {
        actualView = (ViewStates)obj;

        if (actualView == ViewStates.BoardView)
        {
            DisableWordsOfList(InctiveWordsOnBoard);
        }
        else if (actualView == ViewStates.TVView)
        {
            List<WordData> listAllWord = new List<WordData>();
            foreach (GameObject instance in WordsInstances)
            {
                listAllWord.Add(instance.GetComponent<PhoneRowNotebookController>().GetWord());
            }

            DisableWordsOfList(listAllWord);
        }
        else
        {
            List<WordData> Empylist = new List<WordData>();
            DisableWordsOfList(Empylist);
        }
    }

    public void EnableInSlidePhones(Component sender, object obj)
    {
        IsPhoneSlideOut = true;
        List<WordData> Empylist = new List<WordData>();
        DisableWordsOfList(Empylist);
        PutingWordOnBoard(null, null);
    }

    public void DisableInSlidePhones(Component sender, object obj)
    {
        IsPhoneSlideOut = false;
        List<WordData> listAllWord = new List<WordData>();
        foreach (GameObject instance in WordsInstances)
        {
            listAllWord.Add(instance.GetComponent<PhoneRowNotebookController>().GetWord());
        }

        DisableWordsOfList(listAllWord);
    }

    
    void DisableWordsOfList(List<WordData> list)
    {
        foreach (GameObject instanceBTN in WordsInstances)
        {
            instanceBTN.GetComponent<Button>().enabled = true;
            instanceBTN.GetComponent<Button>().interactable = true;

            foreach (WordData word in list)
            {
                PhoneRowNotebookController Wordinstance = instanceBTN.GetComponent<PhoneRowNotebookController>();
                if (Wordinstance.GetWord() == word)
                {
                    instanceBTN.GetComponent<Button>().enabled = false;
                    instanceBTN.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

}
