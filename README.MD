
# Planet Explorator / Procedural Planet Simulator

## Purpose

This document describes the concept and philosophy of the **Procedural Planet Simulator** project.

It is intended for GPT agents or assistant instances that will help with design, architecture, implementation, and future discussions.

This is not a task roadmap.

This is the conceptual foundation of the project.

---

# Core idea

The project is a Unity-based procedural planet simulator.

The target experience is a continuous visual and spatial journey:

```text
a planet is visible far away in space
    -> it occupies only a small part of the screen
        -> the camera approaches it
            -> atmosphere becomes visible
                -> oceans and continents become readable
                    -> mountains, coastlines, and terrain resolve
                        -> the camera enters the sky
                            -> reaches sea level
                                -> can land on the surface
                                    -> can also go underwater
```

The important part is continuity.

The planet seen from orbit must be the same planet reached on the surface.

The terrain must not be replaced by a different local terrain system when the camera gets close.

---

# Project identity

This is not a “noise sphere” project.

The goal is not to generate infinite random detail.

The goal is to generate a believable planet with meaningful large-scale structure, where smaller details appear as part of that structure.

Large forms must dominate small forms.

Small details should decorate and refine the world, not define it.

A bad mental model is:

```text
sphere + many layers of noise = planet
```

The intended mental model is:

```text
planetary structure
    -> continents and oceans
        -> mountain systems
            -> basins and seas
                -> coastlines
                    -> rivers
                        -> local terrain
                            -> rocks, soil, vegetation, small detail
```

Noise is allowed.

Noise is only one tool.

Noise should not be the architecture.

---

# Scale and continuity

The simulator must support the same world at very different scales.

From far away, the planet may occupy only around **3% of the screen area**.

From close range, the camera may stand on the terrain, near water, mountains, beaches, cliffs, or underwater.

These must not be separate scenes.

They must be different views of the same procedural planet.

Conceptually:

```text
height = PlanetFunction(position)
```

The function may become internally complex, layered, cached, streamed, or GPU-evaluated, but the conceptual rule remains:

```text
same position
same planet
same identity
more detail only when needed
```

LOD should change sampling density and visible detail.

LOD should not change what the world fundamentally is.

---

# Terrain principle

Terrain is layered from large to small.

The correct idea is:

```text
continent shape
    + ocean basins
        + mountain systems
            + hills
                + local rocks
                    + micro detail
```

Higher-frequency details are added as the camera approaches.

They must not overwrite or contradict the lower-frequency structure.

A mountain range should not be an accidental result of local noise.

A coastline should not just be where a noisy height field crosses sea level.

A river should not be a random blue line painted over terrain.

The large systems should give meaning to the small systems.

---

# Planet generation hierarchy

The project should be understood as a hierarchy of interacting systems:

```text
planet
    -> tectonic / continental structure
        -> continents / oceans
            -> mountain systems
                -> seas / basins
                    -> coastlines
                        -> rivers
                            -> local terrain
                                -> rocks / vegetation / surface detail
```

Each scale may use its own logic.

Examples:

```text
continents      -> large masks, plate-like organization, low-frequency forms
oceans          -> sea level, basins, shelves, underwater terrain
mountains       -> ridge systems, uplift zones, plate boundaries
coastlines      -> dedicated shoreline shaping
rivers          -> flow networks and drainage logic
beaches         -> slope/height/shoreline-dependent bands
local terrain   -> detail noise, erosion detail, rocks, soil variation
```

The important rule is that these systems should cooperate.

They should not be isolated random layers fighting each other.

---

# Ocean concept

Ocean is a separate planetary layer.

The ocean does not delete terrain.

The ocean does not replace terrain.

The terrain continues below sea level.

Conceptually:

```text
terrain radius = baseRadius + height
ocean radius   = baseRadius + seaLevel
```

If terrain is above sea level, it is land.

If terrain is below sea level, it is seabed.

This gives the simulator:

```text
continents
islands
shallow water
continental shelves
underwater slopes
deep ocean floors
underwater mountains
trenches
```

The water surface is a layer around the planet.

The terrain remains continuous under it.

---

# Underwater concept

The simulator must allow the camera to cross the water surface.

Underwater is not a separate scene.

Underwater is the same planet viewed through water.

When the camera is below sea level, the visual model changes:

```text
water fog
reduced visibility
light attenuation
different color absorption
surface visible above
terrain visible below
```

At first this can be simple.

But the concept must leave room for more complex underwater rendering later.

Important: the underwater mode must be integrated into the same planet framework, not treated as a fake separate environment.

---

# Atmosphere concept

Atmosphere is a core part of the project.

It should not be treated as a rough unrelated approximation or just a skybox.

The intended concept is similar in spirit to the procedural atmosphere shown in the **Coding Adventures** style of videos: atmosphere rendered through post effects / screen-space or camera-based atmospheric scattering logic, where the visual result depends on camera position, planet radius, atmosphere thickness, sun direction, and optical path through the atmosphere.

The first version may be simple.

But it should be built on the same conceptual framework that can later become more complex.

Do not implement a temporary fake atmosphere that must later be thrown away completely.

The correct direction is:

```text
simple atmosphere model
    -> same framework
        -> better scattering
            -> better density profile
                -> better sunset/sunrise behavior
                    -> better altitude transition
                        -> better planet-specific atmosphere parameters
```

The atmosphere should support:

```text
black space outside atmosphere
atmospheric halo around the planet
gradual entry into atmosphere
blue sky near surface
horizon haze
sunset and sunrise color shift
altitude-dependent density
```

The atmosphere is independent from terrain.

A planet profile may have thick atmosphere, thin atmosphere, toxic atmosphere, dense clouds, almost no atmosphere, or other variations.

---

# Clouds concept

Clouds are another layer.

They are not the same thing as the atmosphere, though they visually interact with it.

Clouds may eventually include:

```text
low clouds
high clouds
storm systems
volumetric layers
planet-wide weather patterns
```

Clouds are not the immediate core of terrain generation, but the architecture should leave space for them.

---

# Planet profiles

The simulator should not be hardcoded only for Earth.

The first serious target is Earth-like generation, because it gives the clearest test case:

```text
solid terrain
oceans
atmosphere
sky
coastlines
mountains
underwater transition
```

But the architecture should leave gaps for other planet types.

Future planet profiles may include:

```text
Earth-like planets
Moon-like bodies
Mars-like planets
Venus-like planets
Ice worlds
Ocean worlds
Gas planets
Asteroids
```

Do not design every system as if all planets must have Earth terrain, Earth water, and Earth sky.

At the same time, do not go deep into gas planets or exotic planet types too early.

The important rule is:

```text
Build Earth-like first,
but do not close the architecture against other planet types.
```

For example, gas planets and ice planets should be possible later through different profile logic and specialized layers, but they are not the current focus.

---

# Planet profile parameters

A planet should be driven by profile data.

Possible profile parameters:

```text
radius
gravity
sea level
water amount
temperature
ice amount
atmosphere density
atmosphere composition
atmosphere thickness
tectonic activity
terrain roughness
cloud density
biome rules
```

The profile should influence generation.

It should not merely change colors after generation.

A dry planet, ocean planet, icy planet, or dense-atmosphere planet should differ structurally, visually, and physically.

---

# Surface layer

The surface layer is responsible for the solid body of the planet.

It includes:

```text
continental structure
ocean basins
mountain systems
valleys
coastlines
beaches
cliffs
rivers
biome support
local terrain detail
```

The surface is not only what is above water.

The surface includes seabed and underwater terrain.

---

# Layered planet model

The planet should be understood as layered systems:

```text
solid surface
water / ocean
atmosphere
clouds
entities
```

These layers interact, but they should remain conceptually separate.

Terrain should not contain atmosphere logic.

Atmosphere should not generate terrain.

Ocean should not erase terrain.

Clouds should not be confused with atmosphere.

Entities should come after the planet framework exists.

---

# Technology philosophy

The project is PC-oriented and Unity-based.

The preferred technical direction is:

```text
Unity
C#
Jobs + Burst
Compute shaders where useful
GPU instancing where useful
custom LOD / streaming
```

DOTS/ECS should not be used at the beginning.

DOTS/ECS may become useful later for large numbers of entities:

```text
vegetation
animals
settlements
simulation agents
ecosystems
population systems
```

The early project should focus on the planet itself.

---

# CPU and GPU roles

Heavy parallel work should generally move toward the GPU where appropriate.

Possible GPU-side responsibilities:

```text
terrain height evaluation
normal generation
detail masks
biome masks
atmosphere rendering
cloud rendering
water rendering
local detail generation
```

CPU-side responsibilities:

```text
chunk management
streaming
LOD decisions
planet profile control
save/load state
gameplay logic
high-level orchestration
```

Avoid designs that require constant heavy CPU readback.

The planet should be designed with large-scale GPU-friendly evaluation in mind.

---

# Agent behavior rules

Agents working on this project should respect the conceptual direction.

Do not reduce the project to “add more noise.”

Do not suggest replacing far terrain with unrelated near terrain.

Do not treat atmosphere as a disposable skybox.

Do not treat underwater as a separate fake scene.

Do not make the project Earth-only in its architecture.

Do not jump into vegetation, cities, animals, settlements, or ecosystems before the planetary backbone is stable.

Do not over-focus on gas giants or exotic planets now.

Do leave architectural space for them.

Do not propose DOTS/ECS as the first solution.

Do not write code unless the user asks for code.

When code is requested, use small steps and adapt to the user’s existing structure and names.

The user prefers direct, practical discussion and does not want unnecessary verbosity.

---

# Main validation questions

When proposing ideas, agents should check them against these questions:

```text
Does this preserve one continuous planet from orbit to surface?

Does this add detail instead of replacing the world?

Does this respect large-scale structure?

Does this keep ocean separate from terrain?

Does this keep atmosphere as a real extensible system?

Does this allow underwater transition in the same world?

Does this work when the planet is tiny on screen?

Does this still work when the camera is standing on the surface?

Does this leave space for non-Earth planet profiles later?

Is this conceptually part of the planet framework, or just a temporary fake?
```

---

# Concept summary

Planet Explorator is a layered procedural planet simulator.

The dream is to see a planet as a small distant object in space, approach it, enter its atmosphere, see oceans and continents resolve into real terrain, cross sea level, and stand on the same surface that was visible from orbit.

The planet is not random detail.

The planet is structure first.

Details come later and belong to that structure.

The atmosphere, ocean, underwater mode, terrain, and future planet profiles should all grow from the same framework instead of being disconnected tricks.

The first serious planet can be Earth-like.

But the architecture should remain open enough that later planets can be dry, icy, oceanic, airless, dense-atmosphere, or completely different types such as gas planets.

The core idea is continuity:

```text
one planet
one framework
many layers
many scales
same world
```
