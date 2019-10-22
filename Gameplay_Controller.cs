using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay_Controller : MonoBehaviour
{
    //Bu kod dizisi, açıklamalarda belirttiğim ve play store linkini paylaştığım oyunum 'Zeroo'nun oynanış mekaniğini sağlayan kodları içermektedir.
    //Bu kod dizisi, oyunda yer alan altıgen şekillerin konum,renk gibi bilgilerini barındıran bir text dosyasından aldığı bilgileri kullanarak seviyeyi oluşturur.
    //Seviye oluşturulurken altıgenlerin konumları bir matrix üzerinde düşünülerek konumları tanımlanır.
    //Sonrasında seçili temaya bağlı olarak altıgenlerin ve UI elementlerin renk paletleri belirlenir. 
    //Bulunulan seviyeye bağlı olarak reklam gösterme, müzik ve ses efektlerinin aktif olup olmamasına bağlı olarak ses kliplerinin çalıştırılması gerçekleştirilir.
    //Update fonksiyonunun içerisindeki kod dizisi ise temel oynanışı sağlar, seviyenin tamamlanma durumunu kontol eder.
    //Infinite_level_creator adlı fonksiyon ise belirli bir seviye geçildikten sonra oynanışa uygun rastgele seviyelerin oluşturulabilmesi için gerekli bilgiyi üretir.
    //Oyunu inceleme fırsatı bulursanız.Oynanış ile ilgili daha detaylı bir bilgiye sahip olabilirsiniz.
    //Mehmet Bahadır MAKTAV
    
    public Image bg_hexagon_exp, playable_hexagon_exp, bg_hexagons_panel, playable_hexagons_panel, buttons_panel, level_1_2_info_panel, level210_finished_info_panel;
    public Text level_info_text, hintcounter_info_text;
    public ParticleSystem particle_system;
    private Color[,] playable_objects_colors = { {new Color(0.84f,0.84f,0.84f,1), new Color(0.60f,0,0,1), new Color(0,0.31f,0.86f,1), new Color(1,0.84f,0,1), new Color(0,0.67f,0,1), new Color(0.59f,0,0,0.5f), new Color(0,0.31f,0.86f,0.5f), new Color(1,0.84f,0,0.5f), new Color(0,0.67f,0,0.5f)},
                                                {new Color(0.84f,0.84f,0.84f,1), new Color(0.60f,0.88f,0,1), new Color(0.88f,0.86f,0,1), new Color(1,0.49f,0,1), new Color(0,0.78f,0,1), new Color(0.60f,0.88f,0,0.5f), new Color(0.88f,0.86f,0,0.5f), new Color(1,0.49f,0,0.5f), new Color(0,0.78f,0,0.5f)},
                                                {new Color(0.84f,0.84f,0.84f,1), new Color(0.60f,0,0.39f,1), new Color(0,0.31f,0.39f,1), new Color(1,0.89f,0.20f,1), new Color(0,0.67f,1,1),  new Color(0.60f,0,0.39f,0.5f), new Color(0,0.31f,0.39f,0.5f), new Color(1,0.89f,0.20f,0.5f), new Color(0,0.67f,1,0.5f)},
                                                {new Color(0.84f,0.84f,0.84f,1), new Color(0.60f,0,0.98f,1), new Color(0,0.78f,0.86f,1), new Color(1,0.78f,0,1), new Color(0,0.78f,0,1),  new Color(0.60f,0,0.98f,0.5f), new Color(0,0.78f,0.86f,0.5f), new Color(1,0.78f,0,0.5f), new Color(0,0.78f,0,0.5f)},
                                                {new Color(0.84f,0.84f,0.84f,1), new Color(0.60f,0,0.39f,1), new Color(0,0.31f,0.39f,1), new Color(1,0.89f,0.20f,1), new Color(0,0.67f,1,1),  new Color(0.60f,0,0.39f,0.5f), new Color(0,0.31f,0.39f,0.5f), new Color(1,0.89f,0.20f,0.5f), new Color(0,0.67f,1,0.5f)},
                                                {new Color(0.84f,0.84f,0.84f,1), new Color(0.60f,0,0.98f,1), new Color(0,0.78f,0.86f,1), new Color(1,0.78f,0,1), new Color(0,0.78f,0,1),  new Color(0.60f,0,0.98f,0.5f), new Color(0,0.78f,0.86f,0.5f), new Color(1,0.78f,0,0.5f), new Color(0,0.78f,0,0.5f)}};
    private Color[] colors = new Color[9];
    private Image[,] bg_hexagons, playable_hexagons;
    private int[] loc_x_arr, loc_y_arr, hint_color_arr;
    private bool[,] connected_bool_mat;

    public string inf_level_info = null;
    private void Awake()
    {
        for(int z=0;z<9;z++)
        {
            colors[z] = playable_objects_colors[PlayerPrefs.GetInt("Chosen_Theme"),z];
        } //define objects colors for selected theme
        string[] levels_counter = new string[200];
        string[] location_color_infos = new string[6];
        string[] level_info = new string[37];
        if(PlayerPrefs.GetInt("Current_Level") >= 209)
        {
            level_info = PlayerPrefs.GetString("Last_Infinite_Level").Split('/'); //current level info reading
        }
        else
        {
            var textFile = Resources.Load<TextAsset>("zeroo_level_infos");
            levels_counter = textFile.text.Split('\n');
            level_info = levels_counter[PlayerPrefs.GetInt("Current_Level")].Split('/'); //current level info reading
        }
        if(PlayerPrefs.GetInt("Current_Level") >= 99)
        {
            level_info_text.fontSize = 60;
        }
        if(PlayerPrefs.GetInt("Current_Level") == 0)
        {
            level_1_2_info_panel.transform.GetChild(0).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1.25f,1.25f,1);
            level_1_2_info_panel.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1);
            level_1_2_info_panel.gameObject.GetComponent<Animation>().Play("level1_info_anim");
        }
        else if(PlayerPrefs.GetInt("Current_Level") == 1)
        {
            bg_hexagons_panel.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0,-50,0);
            playable_hexagons_panel.gameObject.GetComponent<RectTransform>().localPosition = new Vector3(0, -50, 0);
            level_1_2_info_panel.transform.GetChild(1).GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.25f, 1);
        }
        level_info_text.text = (PlayerPrefs.GetInt("Current_Level") + 1).ToString();
        hintcounter_info_text.text = PlayerPrefs.GetInt("Hint_Counter").ToString();
        loc_x_arr = new int[level_info.Length];
        loc_y_arr = new int[level_info.Length];
        hint_color_arr = new int[level_info.Length];
        connected_bool_mat = new bool[7, 13];
        bg_hexagons = new Image[7, 13];
        playable_hexagons = new Image[7, 13];
        for (int i = 0; i < level_info.Length; i++)
        {
            location_color_infos = level_info[i].Split(',');
            int x, y, c, n, h;
            bool connected;
            int.TryParse(location_color_infos[0], out x);
            int.TryParse(location_color_infos[1], out y);
            int.TryParse(location_color_infos[2], out c);
            int.TryParse(location_color_infos[3], out n);
            bool.TryParse(location_color_infos[4], out connected);
            int.TryParse(location_color_infos[5], out h);

            bg_hexagons[x, y] = Instantiate(bg_hexagon_exp, bg_hexagons_panel.transform);
            bg_hexagons[x, y].rectTransform.localPosition = new Vector3(62.3815f * y - 374.289f, (340.5f - 113.5f * x));
            bg_hexagons[x, y].gameObject.GetComponent<Animation>().Play("opening_anim");

            playable_hexagons[x, y] = Instantiate(playable_hexagon_exp, playable_hexagons_panel.transform);
            playable_hexagons[x, y].rectTransform.localPosition = new Vector3(62.3815f * y - 374.289f, (340.5f - 113.5f * x) + 5);
            playable_hexagons[x, y].color = colors[c];
            playable_hexagons[x, y].gameObject.GetComponent<Animation>().Play("opening_anim");

            if (n > 0)
            {
                playable_hexagons[x, y].transform.GetChild(0).gameObject.SetActive(true);
                playable_hexagons[x, y].transform.GetChild(0).GetComponent<Text>().text = n.ToString();
                if (c == 1) { counter_hexagons[0] = n; chosen_hex_x[0] = x; chosen_hex_y[0] = y; }
                else if (c == 2) { counter_hexagons[1] = n; chosen_hex_x[1] = x; chosen_hex_y[1] = y; }
                else if (c == 3) { counter_hexagons[2] = n; chosen_hex_x[2] = x; chosen_hex_y[2] = y; }
                else if (c == 4) { counter_hexagons[3] = n; chosen_hex_x[3] = x; chosen_hex_y[3] = y; }
            }
            loc_x_arr[i] = x; loc_y_arr[i] = y; connected_bool_mat[x,y] = connected; hint_color_arr[i] = h;
        }
    }
    private void Start()
    {
        if(PlayerPrefs.GetInt("Current_Level")%5 == 0 && PlayerPrefs.GetInt("Ads_Control_Bool") != 0)
        {
            Ads_Controller.instance.Show_videoad_func();
        }
        if (PlayerPrefs.GetInt("Sound_Control_Bool") == 1)
        {
            GameObject.Find("audio_controller").GetComponents<AudioSource>()[3].Play();
        }
    }

    private int loc_x_now = -5, loc_y_now = -5, loc_x_last = -5, loc_y_last = -5, current_color = -5;
    private int[] counter_hexagons = { 0, 0, 0, 0 };
    private int[] chosen_hex_x = new int[4], chosen_hex_y = new int[4];
    private bool play_check = false, levelfinished = false, check_level_done = false;

    private void Update()
    {

        if (counter_hexagons[0] == 0 && counter_hexagons[1] == 0 && counter_hexagons[2] == 0 && counter_hexagons[3] == 0 && check_level_done == true)
        {
            for(int i = 0; i< loc_x_arr.Length; i++)
            {
                if(connected_bool_mat[loc_x_arr[i], loc_y_arr[i]] == false)
                {
                    levelfinished = false;
                    break;
                }
                else { levelfinished = true; }
            }
            if(levelfinished == true)
            {
                PlayerPrefs.SetInt("Current_Level", PlayerPrefs.GetInt("Current_Level") + 1);
                if(PlayerPrefs.GetInt("Current_Level")> PlayerPrefs.GetInt("Last_Level"))
                {
                    PlayerPrefs.SetInt("Last_Level", PlayerPrefs.GetInt("Current_Level"));
                }
                PlayerPrefs.SetInt("Current_Level", PlayerPrefs.GetInt("Current_Level") - 1);
                if (PlayerPrefs.GetInt("Sound_Control_Bool") == 1)
                {
                    GameObject.Find("audio_controller").GetComponents<AudioSource>()[2].Play();
                }
                particle_system.Play();
                if (PlayerPrefs.GetInt("Current_Level") == 209)
                {
                    level210_finished_info_panel.GetComponent<Animation>().Play("level210_finished_open_anim");
                }
                buttons_panel.gameObject.GetComponent<Animation>().Play("next_button_active_anim");
            }
            check_level_done = false;
        }

        if (Input.touchCount > 0 && levelfinished == false)
        {
            var tp = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            var tp2d = new Vector2(tp.x, tp.y);
            for (int i = 0; i < loc_x_arr.Length; i++)
            {
                if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(tp2d))
                {
                    loc_x_now = loc_x_arr[i]; loc_y_now = loc_y_arr[i];
                    break;
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                for (int i = 0; i < loc_x_arr.Length; i++)
                {
                    if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].GetComponent<CircleCollider2D>() == Physics2D.OverlapPoint(tp2d))
                    {
                        if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[1]) { current_color = 1; play_check = true; break; }
                        else if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[2]) { current_color = 2; play_check = true; break; }
                        else if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[3]) { current_color = 3; play_check = true; break; }
                        else if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[4]) { current_color = 4; play_check = true; break; }
                    }
                }
            }
            if (play_check == true)
            {
                for (int i = 0; i < loc_x_arr.Length; i++)
                {
                    if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[current_color] || playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[current_color + 4])
                    {
                        loc_x_last = loc_x_arr[i]; loc_y_last = loc_y_arr[i];
                        if ((loc_x_last == loc_x_now) && ((loc_y_last + 2 == loc_y_now) || (loc_y_last - 2 == loc_y_now)))
                        {
                            if (playable_hexagons[loc_x_now, loc_y_now].color != colors[1] && playable_hexagons[loc_x_now, loc_y_now].color != colors[current_color + 4]
                                && playable_hexagons[loc_x_now, loc_y_now].color != colors[2] && playable_hexagons[loc_x_now, loc_y_now].color != colors[3]
                                && playable_hexagons[loc_x_now, loc_y_now].color != colors[4])
                            {
                                if(counter_hexagons[current_color - 1] > 0)
                                {
                                    playable_hexagons[loc_x_now, loc_y_now].color = colors[current_color + 4];
                                    counter_hexagons[current_color - 1] -= 1;
                                    playable_hexagons[chosen_hex_x[current_color - 1], chosen_hex_y[current_color - 1]].transform.GetChild(0).GetComponent<Text>().text = counter_hexagons[current_color - 1].ToString();
                                    break;
                                }
                                else { play_check = false;  break; }
                            }
                        }
                        else if ((loc_x_last - 1 == loc_x_now) && ((loc_y_last + 1 == loc_y_now) || (loc_y_last - 1 == loc_y_now)))
                        {
                            if (playable_hexagons[loc_x_now, loc_y_now].color != colors[1] && playable_hexagons[loc_x_now, loc_y_now].color != colors[current_color + 4]
                                && playable_hexagons[loc_x_now, loc_y_now].color != colors[2] && playable_hexagons[loc_x_now, loc_y_now].color != colors[3]
                                && playable_hexagons[loc_x_now, loc_y_now].color != colors[4])
                            {
                                if (counter_hexagons[current_color - 1] > 0)
                                {
                                    playable_hexagons[loc_x_now, loc_y_now].color = colors[current_color + 4];
                                    counter_hexagons[current_color - 1] -= 1;
                                    playable_hexagons[chosen_hex_x[current_color - 1], chosen_hex_y[current_color - 1]].transform.GetChild(0).GetComponent<Text>().text = counter_hexagons[current_color - 1].ToString();
                                    break;
                                }
                                else { play_check = false; break; }
                            }
                        }
                        else if ((loc_x_last + 1 == loc_x_now) && ((loc_y_last + 1 == loc_y_now) || (loc_y_last - 1 == loc_y_now)))
                        {
                            if (playable_hexagons[loc_x_now, loc_y_now].color != colors[1] && playable_hexagons[loc_x_now, loc_y_now].color != colors[current_color + 4]
                                && playable_hexagons[loc_x_now, loc_y_now].color != colors[2] && playable_hexagons[loc_x_now, loc_y_now].color != colors[3]
                                && playable_hexagons[loc_x_now, loc_y_now].color != colors[4])
                            {
                                if (counter_hexagons[current_color - 1] > 0)
                                {
                                    playable_hexagons[loc_x_now, loc_y_now].color = colors[current_color + 4];
                                    counter_hexagons[current_color - 1] -= 1;
                                    playable_hexagons[chosen_hex_x[current_color - 1], chosen_hex_y[current_color - 1]].transform.GetChild(0).GetComponent<Text>().text = counter_hexagons[current_color - 1].ToString();
                                    break;
                                }
                                else { play_check = false; break; }
                            }
                        }
                    }
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                bool touch_ended_sound_control = false;
                for (int i = 0; i < loc_x_arr.Length; i++)
                {
                    if (current_color > 0)
                    {
                        if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[current_color + 4])
                        {
                            playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color = colors[current_color];
                            playable_hexagons[loc_x_arr[i], loc_y_arr[i]].gameObject.GetComponent<Animation>().Play("choice_end_anim");
                            bg_hexagons[loc_x_arr[i], loc_y_arr[i]].gameObject.GetComponent<Animation>().Play("scale_afterchoice_anim");
                            touch_ended_sound_control = true;
                        }
                    }
                }
                for(int z = 0; z < loc_x_arr.Length; z++)
                {
                    for (int i = 0; i < loc_x_arr.Length; i++)
                    {
                        if (connected_bool_mat[loc_x_arr[i], loc_y_arr[i]] == true)
                        {
                            for (int k = 0; k < loc_x_arr.Length; k++)
                            {
                                if ((loc_x_arr[i] == loc_x_arr[k]) && ((loc_y_arr[i] + 2 == loc_y_arr[k]) || (loc_y_arr[i] - 2 == loc_y_arr[k])) && playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == playable_hexagons[loc_x_arr[k], loc_y_arr[k]].color)
                                {
                                    if (connected_bool_mat[loc_x_arr[k], loc_y_arr[k]] == false)
                                    {
                                        connected_bool_mat[loc_x_arr[k], loc_y_arr[k]] = true;
                                    }
                                }
                                else if ((loc_x_arr[i] - 1 == loc_x_arr[k]) && ((loc_y_arr[i] + 1 == loc_y_arr[k]) || (loc_y_arr[i] - 1 == loc_y_arr[k])) && playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == playable_hexagons[loc_x_arr[k], loc_y_arr[k]].color)
                                {
                                    if (connected_bool_mat[loc_x_arr[k], loc_y_arr[k]] == false)
                                    {
                                        connected_bool_mat[loc_x_arr[k], loc_y_arr[k]] = true;
                                    }
                                }
                                else if ((loc_x_arr[i] + 1 == loc_x_arr[k]) && ((loc_y_arr[i] + 1 == loc_y_arr[k]) || (loc_y_arr[i] - 1 == loc_y_arr[k])) && playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == playable_hexagons[loc_x_arr[k], loc_y_arr[k]].color)
                                {
                                    if (connected_bool_mat[loc_x_arr[k], loc_y_arr[k]] == false)
                                    {
                                        connected_bool_mat[loc_x_arr[k], loc_y_arr[k]] = true;
                                    }
                                }
                            }
                        }
                    }
                }
                if(GameObject.Find("audio_controller").GetComponents<AudioSource>()[4].isPlaying != true && touch_ended_sound_control == true && PlayerPrefs.GetInt("Sound_Control_Bool") == 1)
                {
                    GameObject.Find("audio_controller").GetComponents<AudioSource>()[4].Play();
                }
                loc_x_now = -5; loc_y_now = -5; loc_x_last = -5; loc_y_last = -5; play_check = false; current_color = -5; check_level_done = true;
            }
        }
    }
    public void UseHint_button_func()
    {
        if(PlayerPrefs.GetInt("Hint_Counter") > 0 && play_check == false && levelfinished == false)
        {
            int col_info = -5;
            for (int i = 0; i < loc_x_arr.Length; i++)
            {
                for (int z = 0; z < 9; z++)
                {
                    if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[z])
                    {
                        col_info = z;
                        break;
                    }
                }
                if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color != colors[hint_color_arr[i]] && playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color != colors[0])
                {
                    bg_hexagons[loc_x_arr[i], loc_y_arr[i]].transform.GetChild(0).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                    bg_hexagons[loc_x_arr[i], loc_y_arr[i]].transform.GetChild(0).gameObject.GetComponent<Image>().color = colors[0];
                    counter_hexagons[col_info - 1] += 1;
                    playable_hexagons[chosen_hex_x[col_info - 1], chosen_hex_y[col_info - 1]].transform.GetChild(0).GetComponent<Text>().text = counter_hexagons[col_info - 1].ToString();
                    playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color = colors[0];
                    playable_hexagons[loc_x_arr[i], loc_y_arr[i]].gameObject.GetComponent<Animation>().Play("choice_end_anim");
                }
            }
            for (int k = 0; k < 4; k++)
            {
                for (int i = 0; i < loc_x_arr.Length; i++)
                {
                    if (playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color == colors[0])
                    {
                        playable_hexagons[loc_x_arr[i], loc_y_arr[i]].color = colors[hint_color_arr[i]];
                        counter_hexagons[hint_color_arr[i] - 1] -= 1;
                        playable_hexagons[chosen_hex_x[hint_color_arr[i] - 1], chosen_hex_y[hint_color_arr[i] - 1]].transform.GetChild(0).GetComponent<Text>().text = counter_hexagons[hint_color_arr[i] - 1].ToString();
                        playable_hexagons[loc_x_arr[i], loc_y_arr[i]].gameObject.GetComponent<Animation>().Play("choice_end_anim");
                        break;
                    }
                }
            }
            PlayerPrefs.SetInt("Hint_Counter", PlayerPrefs.GetInt("Hint_Counter") - 1);
            hintcounter_info_text.text = PlayerPrefs.GetInt("Hint_Counter").ToString();
            if (PlayerPrefs.GetInt("Sound_Control_Bool") == 1)
            {
                GameObject.Find("audio_controller").GetComponents<AudioSource>()[1].Play();
                GameObject.Find("audio_controller").GetComponents<AudioSource>()[4].Play();
            }
        }
    }
    public void Infinite_level_creator()
    {
        int[] x_arr = { 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3, 3, 3, 4, 4, 4, 4, 4, 4, 5, 5, 5, 5, 5, 6, 6, 6, 6 };
        int[] y_arr = { 3, 5, 7, 9, 2, 4, 6, 8, 10, 1, 3, 5, 7, 9, 11, 0, 2, 4, 6, 8, 10, 12, 1, 3, 5, 7, 9, 11, 2, 4, 6, 8, 10, 3, 5, 7, 9 };
        int[] used = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] c_arr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] n_arr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        bool[] connected = { false, false ,false ,false,false,false,false,false,false,false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
        int[] h_color = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[,] color_set = { { 4, 2, 1 }, { 3, 1, 2 }, { 2, 3, 4 } };

        int rand_color_set = Random.Range(0,3);
        for(int z=0;z<3;z++)
        {
            int n_arr_counter = 0;
            int rand_counter = Random.Range(5,12);
            int rand_hex = 0;
            while(true)
            {
                rand_hex = Random.Range(0,37);
                if(used[rand_hex] == 0)
                {
                    used[rand_hex] = 1;
                    c_arr[rand_hex] = color_set[rand_color_set, z];
                    connected[rand_hex] = true;
                    h_color[rand_hex] = color_set[rand_color_set, z];
                    break;
                }
            }
            for(int z1=0;z1<rand_counter;z1++)
            {
                for(int z2=0;z2<500;z2++)
                {
                    bool loop_breaker = false;
                    int counter = Random.Range(0, 37);
                    if(used[counter] == 0)
                    {
                        for (int z3 = 0; z3 < 37; z3++)
                        {
                            if (used[z3] == 1 && h_color[z3] == color_set[rand_color_set, z])
                            {
                                if ((x_arr[z3] == x_arr[counter]) && ((y_arr[z3] + 2 == y_arr[counter]) || (y_arr[z3] - 2 == y_arr[counter])))
                                {
                                    used[counter] = 1;
                                    h_color[counter] = color_set[rand_color_set, z];
                                    loop_breaker = true;
                                    break;
                                }
                                else if ((x_arr[z3] - 1 == x_arr[counter]) && ((y_arr[z3] + 1 == y_arr[counter]) || (y_arr[z3] - 1 == y_arr[counter])))
                                {
                                    used[counter] = 1;
                                    h_color[counter] = color_set[rand_color_set, z];
                                    loop_breaker = true;
                                    break;
                                }
                                else if ((x_arr[z3] + 1 == x_arr[counter]) && ((y_arr[z3] + 1 == y_arr[counter]) || (y_arr[z3] - 1 == y_arr[counter])))
                                {
                                    used[counter] = 1;
                                    h_color[counter] = color_set[rand_color_set, z];
                                    loop_breaker = true;
                                    break;
                                }
                            }
                        }
                    }
                    if(loop_breaker == true)
                    {
                        break;
                    }
                }
            }

            for (int z5 = 0; z5 < 37; z5++)
            {
                if(h_color[z5] == color_set[rand_color_set, z])
                {
                    n_arr_counter += 1;
                }
            }
            if(n_arr_counter > 2)
            {
                while (true)
                {
                    int z5 = Random.Range(0, 37);
                    if (h_color[z5] == color_set[rand_color_set, z] && z5 != rand_hex)
                    {
                        c_arr[z5] = color_set[rand_color_set, z];
                        n_arr_counter -= 1;
                        break;
                    }
                }
            }
            if (n_arr_counter > 1)
            {
                n_arr[rand_hex] = n_arr_counter - 1;
            }
            else
            {
                used[rand_hex] = 0;
            }
        }



        for(int z4=0;z4<37;z4++)
        {
            if(used[z4] == 1)
            {
                inf_level_info += x_arr[z4].ToString() + ',' + y_arr[z4].ToString() + ',' + c_arr[z4].ToString() + ',' + n_arr[z4].ToString() + ',' + connected[z4].ToString() + ',' + h_color[z4].ToString() + '/';
            }
        }
        inf_level_info = inf_level_info.Substring(0, inf_level_info.Length - 1);
        PlayerPrefs.SetString("Last_Infinite_Level", inf_level_info);
    }
}
