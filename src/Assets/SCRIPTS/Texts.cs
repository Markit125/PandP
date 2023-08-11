using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class Texts : MonoBehaviour
{
    private Vector3 pos, scl;
    public int num, count, numpage, favnum, lastfav;
    public TextAsset tex, ortex;
    public GameObject Head, Page, Star;
    private Text txt, htx, pg;
    private string[] strs, originals, fs;
    private float timeClick;
    public List<int> fav;
    private bool ViewFav, ViewOri;
    public Button prev, next;

    void Awake()
    {
        Star.GetComponent<Animation>()["Favor"].speed = 10000f;
        Star.GetComponent<Animation>().Play("Favor");
        StartCoroutine("StarPos");
        numpage = 0; num = 0; favnum = 0; ViewFav = false; lastfav = 0;
        pg = Page.GetComponent<Text>();
        txt = GetComponent<Text>();
        htx = Head.GetComponent<Text>();
        strs = tex.text.Split('\n');
        originals = ortex.text.Split('\n');
        count = strs.Length;
        txt.text = strs[num].Substring(6);
        PorP(strs[num], htx);

        if (!File.Exists(Application.persistentDataPath + @"/fav.txt"))
        {
            StreamWriter sw = new StreamWriter(Application.persistentDataPath + @"/fav.txt");
            sw.Close();
        }
        List<string> lines = File.ReadAllLines(Application.persistentDataPath + "/fav.txt").ToList();
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i] == "") lines.RemoveAt(i);
        }
        for (int i = 0; i < lines.Count; i++)
        {
            if (int.TryParse(lines[i].Substring(2, 3), out _)) fav.Add(int.Parse(lines[i].Substring(2, 3)));
        }
        File.WriteAllLines(Application.persistentDataPath + "/fav.txt", lines.ToArray());
        InFav();

        pg.text = numpage.ToString();
    }

    public void PrevClick()
    {
        Star.GetComponent<Animation>().Stop("UnFavor");
        if (!ViewFav)
        {
            if (num == 0)
            {
                num = count - 1;
            }
            else
            {
                num -= 1;
            }
            numpage -= 1;
            if (numpage == -count) numpage = 0;
            pg.text = numpage.ToString();
            txt.text = strs[num].Substring(6);
            PorP(strs[num], htx);
            normal();
            InFav();
        }
        else
        {
            if (fav.Count > 0)
            {
                normal();
                Star.GetComponent<SpriteRenderer>().enabled = true;
                if (favnum <= 0)
                {
                    favnum = fav.Count - 1;
                }
                else
                {
                    favnum -= 1;
                }
                lastfav = fav[favnum];
                pg.text = fav[favnum].ToString();
                txt.text = strs[fav[favnum]].Substring(6);
                PorP(strs[fav[favnum]], htx, "Избранная");
            }
            else
            {
                normal();
                Star.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }
    public void NextClick()
    {
        Star.GetComponent<Animation>().Stop("UnFavor");
        if (!ViewFav)
        {
            if (num == count - 1)
            {
                num = 0;
            }
            else
            {
                num += 1;
            }

            numpage += 1;
            if (numpage == count) numpage = 0;
            pg.text = numpage.ToString();
            txt.text = strs[num].Substring(6);
            PorP(strs[num], htx);
            normal();
            InFav();
        }
        else
        {
            if (fav.Count > 0)
            {
                normal();
                Star.GetComponent<SpriteRenderer>().enabled = true;
                if (favnum >= fav.Count - 1)
                {
                    favnum = 0;
                }
                else
                {
                    favnum += 1;
                }
                lastfav = fav[favnum];
                pg.text = fav[favnum].ToString();
                txt.text = strs[fav[favnum]].Substring(6);
                PorP(strs[fav[favnum]], htx, "Избранная");
            }
            else
            {
                normal();
                Star.GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    void OnMouseDown()
    {
        if (Time.time - timeClick < 0.5f)
        {
            if (!ViewFav || !(fav.Count == 0))
            {
                bool x = true;
                if (!ViewFav)
                {
                    for (int i = 0; i < fav.Count; i++)
                    {
                        if (num == fav[i])
                        {
                            x = false;
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < fav.Count; i++)
                    {
                        if (lastfav == fav[i])
                        {
                            x = false;
                            break;
                        }
                    }
                }
                if (x)
                {
                    // Insert in fav
                    InsertFav();
                    if (!ViewFav) fav.Add(num);
                    else fav.Add(lastfav);

                    using (StreamWriter sw = File.AppendText(Application.persistentDataPath + "/fav.txt"))
                    {
                        if (!ViewFav) sw.WriteLine(strs[num]);
                        else sw.WriteLine(strs[lastfav]);
                    }
                    List<string> lines = File.ReadAllLines(Application.persistentDataPath + "/fav.txt").ToList();
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i] == "") lines.RemoveAt(i);
                    }
                    File.WriteAllLines(Application.persistentDataPath + "/fav.txt", lines.ToArray());
                }
                else
                {
                    // Remove from fav
                    RemoveFav();
                    List<string> lines = File.ReadAllLines(Application.persistentDataPath + "/fav.txt").ToList();
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (!ViewFav)
                        {
                            if (int.Parse(lines[i].Substring(2, 3)) == num)
                            {
                                lines.RemoveAt(i);
                            }
                        }
                        else
                        {
                            if (int.Parse(lines[i].Substring(2, 3)) == lastfav)
                            {
                                lines.RemoveAt(i);
                            }
                        }
                    }
                    for (int i = 0; i < lines.Count; i++)
                    {
                        if (lines[i] == "") lines.RemoveAt(i);
                    }
                    File.WriteAllLines(Application.persistentDataPath + "/fav.txt", lines.ToArray());
                    if (!ViewFav) fav.Remove(num);
                    else fav.Remove(lastfav);
                }
            }
            else if (ViewFav && fav.Count == 0 && htx.text != "Избранное")
            {
                Star.GetComponent<Animation>().Play("Favor");
                Star.GetComponent<SpriteRenderer>().enabled = true;
                fav.Add(lastfav);
                using (StreamWriter sw = File.AppendText(Application.persistentDataPath + "/fav.txt"))
                {
                    sw.WriteLine(strs[fav[favnum]]);
                }
                List<string> lines = File.ReadAllLines(Application.persistentDataPath + "/fav.txt").ToList();
                for (int i = 0; i < lines.Count; i++)
                {
                    if (lines[i] == "") lines.RemoveAt(i);
                }
                File.WriteAllLines(Application.persistentDataPath + "/fav.txt", lines.ToArray());
            }
            timeClick = 0;
        }
        else timeClick = Time.time;
    }

    void InFav()
    {
        normal();
        bool x = true;
        for (int i = 0; i < fav.Count; i++)
        {
            if (num == fav[i])
            {
                x = false;
                break;
            }
        }
        if (x) Star.GetComponent<SpriteRenderer>().enabled = false;
        else Star.GetComponent<SpriteRenderer>().enabled = true;
    }

    public void FavoriteMode()
    {
        if (ViewOri) Originals();
        ViewFav = !ViewFav;
        if (ViewFav)
        {
            if (fav.Count > 0)
            {
                lastfav = fav[favnum];
                if (favnum > fav.Count - 1) favnum = fav.Count - 1;
                else if (favnum < 0) favnum = 0;
                pg.text = fav[favnum].ToString();
                txt.text = strs[fav[favnum]].Substring(6);
                PorP(strs[fav[favnum]], htx, "Избранная");
                InFav();
                normal();
                Star.GetComponent<SpriteRenderer>().enabled = true;
            }
            else
            {
                htx.text = "Избранное"; txt.text = "Добавляйте в избранное двойным нажатием на пословицу или поговорку"; pg.text = "";
                Star.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        else
        {
            txt.text = strs[num].Substring(6);
            PorP(strs[num], htx);
            InFav();
            pg.text = numpage.ToString();
        }
    }
    
    public void Originals()
    {
        ViewOri = !ViewOri;
        if (ViewOri)
        {
            prev.GetComponent<Animation>().Play("Little");
            next.GetComponent<Animation>().Play("Little");
            if (ViewFav)
            {
                htx.text = "Original\n" + htx.text.Substring(10);
                bool b = false;
                for (int i = 0; i < fav.Count; i++)
                {
                    if (favnum == fav[i])
                    {
                        b = true;
                        break;
                    }
                }
                if (b) txt.text = originals[fav[favnum]].Substring(6);
                else txt.text = originals[lastfav].Substring(6);
            }
            else
            {
                htx.text = "Original" + htx.text;
                txt.text = originals[num].Substring(6);
            }
        }
        else
        {
            prev.GetComponent<Animation>().Play("Huge");
            next.GetComponent<Animation>().Play("Huge");
            if (ViewFav)
            {
                PorP(strs[fav[favnum]], htx, "Избранная");
                bool b = false;
                for (int i = 0; i < fav.Count; i++)
                {
                    if (favnum == fav[i])
                    {
                        b = true;
                        break;
                    }
                }
                if (b) txt.text = strs[fav[favnum]].Substring(6);
                else txt.text = strs[lastfav].Substring(6);
            }
            else
            {
                PorP(strs[num], htx);
                txt.text = strs[num].Substring(6);
            }
        }
    }
    private void PorP(string s, Text h, string dop = "")
    {
        if (s[0] == '0') h.text = dop + "\nПословица";
        else h.text = dop + "\nПоговорка";
    }
    private void RemoveFav()
    {
        Star.GetComponent<SpriteRenderer>().enabled = true;
        Star.GetComponent<Animation>().Play("UnFavor");
    }
    private void InsertFav()
    {
        Star.GetComponent<SpriteRenderer>().enabled = true;
        Star.GetComponent<Animation>().Play("Favor");
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape)) Application.Quit();
    }

    IEnumerator StarPos()
    {
        yield return new WaitForSeconds(0.05f);
        Star.GetComponent<Animation>()["Favor"].speed = 1f;
        pos = Star.GetComponent<Transform>().position;
        scl = Star.GetComponent<Transform>().localScale;
    }

    private void normal()
    {
        Star.GetComponent<Transform>().position = pos;
        Star.GetComponent<Transform>().localScale = scl;
    }
}