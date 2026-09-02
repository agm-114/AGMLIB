# Missile track relay support

`ModularTrackRelaySupportDescriptor` is a modular-missile support component that
publishes contacts acquired by the missile's autonomous seekers to the friendly
sensor network.

The descriptor finalizes the missile with a `SensorHost`, `Communicator`, an
`ICommsAntenna`, and the pooled runtime relay. When the missile launches, it
creates a missile-owned sensor context after ownership and network identity are
ready. That context uses the missile's position as the observation origin and
is destroyed when the missile dies or returns to its pool.

## Configuration

Create a `ModularTrackRelaySupportDescriptor` asset from
`Nebulous > Missiles > Support > Modular Track Relay`, configure its ordinary
support-component identity and cost, and make it available to the intended
support socket.

The relay-specific fields are:

- **Acquisition Type** controls how every relayed contact contributes to the
  friendly track: `Active` produces an ordinary tracked contact, `Ping` produces
  a periodically refreshed ping, and `Passive` produces a bearing-only contact.
- **Reported Signature Type** controls the sensor/signature category attributed
  to the relay contribution.
- **Collects Intel** allows the relay contribution to perform native
  identification work. Leave it disabled for a cheap tripwire that only reports
  position or bearing data.

The descriptor reuses an existing communicator or antenna on the finalized
missile. If either is absent, it adds a native `Communicator` and a default
`SimpleCommsAntenna`.

## Track boundary

Only missile-local `SensorTrack` instances are relayed. In native terms, these
are seeker tracks whose `SensorTrack.Context` is `null`, such as contacts held by
active or passive onboard seekers. Command and datalink seekers consume tracks
from another sensor context, so the relay deliberately ignores them instead of
feeding an already-networked track back into itself.

Multiple seekers that currently detect the same object produce one relay
contribution. When every autonomous seeker loses or switches away from that
object, the relay removes its contribution on the next sensor acquisition
cycle.

## Runtime limitations

- Track sharing depends on the missile communicator remaining connected to the
  friendly sensor network. Communications jamming can isolate the missile
  context even if its seeker still sees the target locally.
- The relay copies the source seeker's known position and velocity; it does not
  reveal the target's authoritative true position.
- The support component does not make a seeker search. It only reports tracks
  that normal missile guidance or validation has already acquired.
- Use a missile lifetime or loitering behavior appropriate for a deployable
  sensor. The relay stops as soon as the missile dies or is repooled.
