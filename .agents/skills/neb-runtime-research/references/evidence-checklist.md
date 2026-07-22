# Runtime Evidence Checklist

Use only the sections relevant to the question.

## Identity And Version

- Record the game DLL path, file version when available, last-write time, and SHA-256.
- Compare the live game assembly with `AGMLIB/libs` when a user may have an old DLL.
- Identify the exact bundle and prefab address, not only the display name.
- Follow the actual spawned prefab reference through warheads, factories, catalogs, or pools.

## Type And Dispatch

- List the inheritance chain and declaring type for the member.
- Distinguish `override`, inherited implementation, `new` method hiding, Harmony replacement, and Unity message dispatch.
- Verify whether the callback is private/protected/public and whether Unity invokes it by message name.
- Check every overload and generic parameter before selecting a patch target.

## Serialized State

- Inspect the serialized reference and its target component.
- Treat code initializers as fallbacks, not proof of a prefab value.
- Check old prefab/script identity and missing MonoScript references when a DLL or `.meta` GUID changed.
- Do not save a prefab showing missing scripts until the identity mismatch is understood and a backup exists.
- For finalized, spawned, or pooled objects, verify required private fields are `[SerializeField]` or rebuilt on the live instance.

## Lifecycle

- Name the state at construction, finalization, launch, pooling/unpooling, arming, activation, destruction, and UI detail rendering.
- Record whether a field is valid at each state.
- Check delayed initialization and bounded readiness conditions.
- Confirm whether submunitions take a different launch path from directly launched munitions.

## Physics And Targets

- Verify the linked collider exists, is enabled, and is a trigger at the relevant state.
- Check the GameObject layer and the active physics collision matrix.
- Confirm a Rigidbody/Collider arrangement capable of producing trigger callbacks.
- Trace how `other` is resolved: collider object, parent, root, attached Rigidbody, interface, or concrete component.
- Test ship, fighter/spacecraft, missile, mine, friendly, enemy, and obstructed cases separately when supported.
- Separate wake/activation targeting from the seeker or fuse logic that runs after activation.

## Authority And Ownership

- State whether the callback runs on host/server, client, or both.
- Verify team/friendly filtering and communications exceptions.
- Prefer host-owned mutation and vanilla replication.
- In multiplayer, compare both logs and identify which role emitted each line.

## Conclusion Quality

- Cite the exact methods and serialized fields supporting the conclusion.
- Separate confirmed facts from inference.
- Explain why close alternatives do not apply.
- Propose the smallest test that can falsify the remaining hypothesis.
