using System.Collections.Generic;

namespace MazeGuy
{
    public partial class Game1
    {
        // Embedded levels - stored as string arrays
        private static readonly Dictionary<int, string[]> Levels = new Dictionary<int, string[]>
        {
            { 1, new string[] {
                "s      #   #   ####   #    ######    ##      #    #  #",
                "#####  # # # # #### # #  #      #         #  #    #  #",
                "       # # # #      #         #     ##    #  #  # #  #",
                "  ###### # # ############   #    #        #  #  # #  #",
                "         # #    #             #           #  #  # #  #",
                "########## ###  #   #  ###  ##### ##      #  #  # #  #",
                "         #      #  #  #            #####  #  #  #    #",
                " ################  # #                  ###########  #",
                "                     #    ###############            #",
                "#############  #######                     ###########",
                "                        ###########                  #",
                "   ############################       ############   #                     ",
                "                            ###############          #",
                "####################          #      #        ########",
                "                              #  #   #               #",
                "   #########################  #  #   #############   #",
                "                                 #                   #",
                "########  #############          #####################",
                "                #   # #  ####                        #",
                "#############   #   # #  #     ###################   #",
                "                    # #  #  #               #    #   #",
                "   ################## #  #  ##############  #  # #   #",
                "                      #  #  #               #  # #   #",
                "#######################  #  ##########  #####  # #####",
                "                                               #     x",
                "######################################################"
            }},
            { 2, new string[] {
                "siiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiidddddddddddddddddddddddddddddddddddddddoooooooooooooooooo33333,,,,,,;[['['nbbvdfdgffdddddddddddddddmmmmm,m,mmmmmnbbffgtyutrr",
                "iiiiiiiiiiiiiiiiiiiiigggggggggggggggggggggggggggeqqqttttoiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiim"
            }},
            { 3, new string[] {
                "siiiiiiiiiiiiiiiiiiiiiooooooooooooooooooooooooooooooooooooooooooooooooooxooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooooo",
                "                                                                        do",
                "                                                                        do",
                "oooooooooooddddddddddiiiiiiiiiiiiiiiiiiiiiiididddddddddoooo             do",
                "oooooooooooooooiooooooooooooooooooooooooiooooooodoooooooooo             do",
                "oooooooooooodddoooooooooooooooooooooooodiddddddddddddddddoxd            do                        o",
                "ddddddddddddddddddddddddddiiiiddddddddddiiiioiiiiioiiiiidddddddddddddddddo ",
                "dddddddddiiiddddddddddddd_iiiioooooodooooodiiiddoiiiiioiiddddddddddddddddo   ",
                "ooooooooxoooooxooooooooooooooooxooooooooooooxooooooooooooooooooooooooooooo"
            }},
            { 4, new string[] {
                "eeeeexniiiiiiiiiiisssssssfsdrdddddettreeeeeewqqqsssseiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiiit",
                "",
                "",
                "",
                "",
                "",
                "gfdsesfrtefiieeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeweee5767ertyer65r4565xxcdvgbhmkjkjukiiiikiuiytytyuyujuhjmnh,jnjhfjkfhvjkbfvjk"
            }}
        };
    }
}
