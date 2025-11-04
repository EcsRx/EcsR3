## EcsR3.Plugins.UtilityAI

While there isn't as much information available as I would like about Utility/Infinite Axis style AI, there is enough if you google it so I wont go over that too much here, but here is a quick crash course:

### Quick Utility/Infinite Axis AI Overview

The high level idea is that you distill contextual information into normalized values of 0-1 which are then used together to score how good high level actions are.

For example if you have a player with data like `{ health: number, maxHealth: number, ammo: number, position: vector3 }` you can normalize a lot of these values so if you get the health percentage as a normalized value 0-1 then you can use that as a `Consideration` when factoring in if you should carry out `Actions` such as if you should take cover or shoot an enemy (assuming you also have enough ammo).

With this in mind we have 2 high level concepts:

- `Considerations` - These are normalized numeric abstractions (0-1) over contextual information, such as the health, ammo levels, distance to threats etc
- `Actions` - These are higher level actions your agent may want to do based on the `Considerations` it is given, it decides how good an action is by scoring all related `Considerations` to provide a feel as to how useful the action is (aka its utility).

With this being said there is a bit more complexity at play under the hood, as the name `Utility` comes from how useful something is, and having a `Consideration` between 0-1 is good, but we need to provide some context as to "how good" that consideration is, which is where `Curve Functions` come in as they allow us to plot our value to a final consideration.

> The OpenRpg lib that this plugin uses for curve functions and variables, has a page that explains curve functions in more depth and has its own AI related examples/demos, but while at the high level that library is doing same thing as this its API is quite different. https://openrpg.github.io/OpenRpg/

To give a more detailed example, if we think about 2 high level actions `Attack` and `TakeCover`, the `Attack` action would likely only be a good idea if we have a good amount of health (ignoring ammo levels for now), and the `TakeCover` action would likely be important if health was low, so if we have 75% health (expressed as 0.75f) is that good or not, for Attacking yeah 0.75f seems like a high score, but for Taking cover 0.75f also seems like a high score, so should both actions be as likely at 75% health? (no they shouldnt).

So jumping back to curve functions, lets create 2 `Considerations` based off the same underlying input (Health Percentage):

`Healthy` Consideration

<img width="350" height="234" alt="image" src="https://github.com/user-attachments/assets/7502e559-a022-47dd-bfa0-e0fad8d2c367" />

If we pass in 0.75f to the linear function we will get out 0.75f which is fine, as it makes sense as we are quite healthy.

`LowHealth` Consideration

<img width="346" height="214" alt="image" src="https://github.com/user-attachments/assets/2326da26-f437-467a-b801-ea4c8292e526" />

Notice how we have an inverse linear function, which is the opposite, so passing in 0.75f would output 0.25f, meaning that its not actually that high at the moment.

Taking the 2 considerations together, we can drive the `Attack` action from `Health` consideration, and the `Take Cover` action from the `LowHealth` consideration.

You could even get fancier and express `LowHealth` with a more drastic curve where anything above 50% health is seen as fine, its only when you get to less than 50% health it DRASTICALLY ramps up
<img width="337" height="218" alt="image" src="https://github.com/user-attachments/assets/1788e684-b698-479e-9af4-2e8770ea5428" />

Either way the input value alone needs to be run through a curve function to plot how high the value actually is, so the same inputs can be considered multiple ways.

With this taken care of now if we have 75% health our Attack would score 0.75f and TakeCover would score far lower at 0 (on latter graph).

Just before we finish off there is one more thing to mention, which is how `Actions` can factor in multiple `Considerations` so in the above example its fine having a high `Health` consideration, but we would also need to ensure we had `Ammo` and were close enough for our weapon to hit an enemy, or if there is even an enemy there.

For example if we had 75% health, and an enemy pretty close, if we have no ammo we cant shoot, so taking imaginary consideration scores like:
- `Health` 0.75f
- `EnemyNearby` 0.75f
- `Ammo` 0f

It doesnt matter that 2 of the `Considerations` are high, as the ammo one is 0, which acts as a sort of veto and will mean when they are calculated together as when multiplied by 0 its going to be 0, which makes the action a no go. Also as the `Considerations` are all normalized between 0-1 you can use as many of them as you want, which is where the `Infinite Axis` name comes from, you can add as many axis/considerations as you want to score your actions, which makes it a very additive system as you come up with newer `Considerations` you may want to factor them into your `Actions`.

> The `Action` score is not quite as simple as multiplying all the normalized `Considerations` together, as the numbers would continue to shrink the more you have. So there is a compensator built into the calculation to stop this, so the numbers will remain accurate, so you cant just Average, or Sum and Divide etc.

Finally worth mentioning we use the term `Advice` rather than `Action` as we split the scoring and doing of the `Action` into different things, so the scoring part of an action is the `Advice` and we leave you to do the actual `Action` execution elsewhere.

### Why Utility AI and not GOAP/FSM/BT/MGS etc

Metal Gear Solid while being a fine game would not fit here, but the other paradigms they are all good solutions to the problem of "how to allow entities to decide what to do", and in some situations you may want to use more complex task planners or simpler state machines to handle actions etc, but the cool thing about Utility AI is that at the heart of it, it just provides you information to do what you want with.

Utility AI excels at being additive and emergent while not causing causing headaches, new `Considerations` have no impact on existing ones, and can be added to existing `Actions` without any concern as they are all just normalized numbers.

Also the focus is less on modelling states and transitions, and instead modelling possible `Actions` and advising on how useful they would be. Meaning there is no hard transitions or states as such, you just have a lot of weighted information and you can decide to do one or many `Actions` at a given time.

For example you may want to break your `Actions` down into `Categories` (aka `Buckets`) so you could have movement based actions and interaction based actions, which could run alongside each other, maybe you are running to cover while shooting nearest enemy, or healing while running towards a flag to capture.

With all the above said it has a lot of similarities with the ECS paradigm in how it is extensible and non breaking when you are iterating and adding new things, it can also be used with other approaches if you wanted to use FSMs with it you could just have your transitions/states look at the scores and then let it do its state flows.

Ultimately its flexible and easy to use directly or use in conjunction with other approches, so thats why we are using that as the first out the box AI approach.

### Ok thats enough, how do we use it?

The plugin contains a simple implementation of the above paradigm, mainly focusing on:

- `AgentComponent` - Component containing scores for considerations/actions
- `ConsiderationSystem` - Handles recalculating scores for considerations (i.e `AmmoRemaining`, `LowHealth` etc)
- `AdviceSystem` - Handles taking multiple considerations and calculating a total score for a given action (i.e `ShouldHealSelf`, `ShouldAttack` etc)

These 3 things alone will provide you with a beating heart of scores telling you what actions have the most utility at a given time, as its all driven by `BatchedSystems` under the hood you can schedule them however you want, however these systems only deal with giving you the numeric scoring and the **DOING** of the action is left for you to do separately, however we do provide some helper systems you can use such as:

- `AdviceActionSystem` - This provides a basic system that can execute logic for a given Advice/Action
- `AdviceRankingSystem` - This is a simple ranking system that periodically ranks the advice, as by default you just get lots of scored advice and would need to rank them yourself, but this is loaded by default with the plugin.

### I want to know more about Utility/Infinite Axis AI

Your best bet is to start here, where the main guy who came up with the concept (Dave Mark) discusses it and mentions his books and other previous lectures/presentations on the subject.

[https://www.gdcvault.com/play/1021848/Building-a-Better-Centaur-AI](https://www.gdcvault.com/play/1021848/Building-a-Better-Centaur-AI)
