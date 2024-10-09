using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotebookPhonesController : MonoBehaviour
{
    [SerializeField] GameObject PhoneNumberPrefab;
    [SerializeField] Transform WordContainer;
    List<GameObject> WordsInstances = new List<GameObject>();
    List<int> removedIndex = new List<int>(); // Lista para almacenar los índices eliminados
    int i = 0;
    bool once = false;

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

        if (LastPhoneAdded.GetIsPhoneNumberFound())
        {
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

        GameObject wordaux = Instantiate(PhoneNumberPrefab, WordContainer);
        wordaux.GetComponent<Button>().onClick.AddListener(ClearUnderLine);
        wordaux.GetComponent<PhoneRowNotebookController>().Initialization(LastPhoneAdded);
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

    // Función para reemplazar un número por otro
    bool WordReplaceOther(WordData newword)
    {
        bool aux = false;
        foreach (GameObject w in WordsInstances)
        {
            if (!newword.GetWordThatReplaces()) continue;
            PhoneRowNotebookController script = w.GetComponent<PhoneRowNotebookController>();
            if (script.GetWord() == newword.GetWordThatReplaces())
            {
                script.ReplaceNumber(newword);
                ClearUnderLine();
                aux = true;
            }
        }
        return aux;
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

    public void DisablePhoneBTN(Component sender, object obj)
    {
        /*if (obj is not bool) return;

        if (WordContainer.childCount == 0) return;
        GameObject[] btns = WordContainer.gameObject.GetComponentsInChildren<GameObject>();
        
        foreach (GameObject btn in btns)
        {
            btn.SetActive(!(bool)obj);
        }*/
    }
}
