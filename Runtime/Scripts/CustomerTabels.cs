using System.Collections.Generic;
using UnityEngine;

public class CustomerTabels : MonoBehaviour
{
    [SerializeField] private CustomerLines m_customerLines;

    [SerializeField] private Tabel[] m_tabels;

    private void Start()
    {
        for (int i = 0; i < m_tabels.Length; i++)
        {
            m_tabels[i].CustomerLines = m_customerLines;
        }
    }

    public bool CustomerTabel(out Tabel tabel, out int tabelIndex)
    {
        List<int> openTabelIndexes = new List<int>();
        for (int i = 0; i < m_tabels.Length; i++)
        {
            if (!m_tabels[i].TabelTaken)
            {
                openTabelIndexes.Add(i);
            }
        }

        if (openTabelIndexes.Count == 0)
        {
            tabel = new Tabel();
            tabelIndex = -1;
            return false;
        }

        tabelIndex = openTabelIndexes[Random.Range(0, openTabelIndexes.Count)];
        tabel = m_tabels[tabelIndex];
        return true;
    }
}
