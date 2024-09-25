
 Hello! Are you Mr.Owl?
 Yes, I am!
 Sorry, recently the magic tree has been weak so I need a lot of sleep to recharge my energy
 
 -> Divert1
VAR asked_magic_tree = false
VAR asked_queen = false
VAR asked_about_me = false

== Divert1 ==
Do you need any help?
* {not asked_magic_tree} [About the magic tree] -> MagicTree
* {not asked_queen} [About the Queen] -> Queen
* {not asked_about_me} [About me] -> AboutMe

== MagicTree ==
The magic tree gives light to the forest and protects the forest
~ asked_magic_tree = true
{asked_magic_tree && asked_queen && asked_about_me:
    -> End
- else:
    * [Go back] -> Divert1
}

== Queen ==
The Queen was originally a beautiful woman, but she was too obsessed with beauty
~ asked_queen = true
{asked_magic_tree && asked_queen && asked_about_me:
    -> End
- else:
    * [Go back] -> Divert1
}

== AboutMe ==
You are a small flower bud, it seems you have gained a bit of the power of the magic tree and have become a spirit
~ asked_about_me = true
{asked_magic_tree && asked_queen && asked_about_me:
    -> End
- else:
    * [Go back] -> Divert1
}

== End ==
Thank Mr.Owl!
-> END