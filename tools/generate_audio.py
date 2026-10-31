#!/usr/bin/env python3
"""Original deterministic sound synthesis. Python standard library; no samples/downloads.
Writes only the three named WAVs; --check verifies them without changing any file.
"""
import argparse
import hashlib
import json
import math
from pathlib import Path
import random
import struct
import wave

RATE = 22050
ROOT = Path(__file__).resolve().parents[1] / 'UnityAssets/SonBirUmutDemo/Audio'
SPECS = {'HopefulMenu.wav': 12, 'VillageWind.wav': 8, 'CrisisUndertone.wav': 8}


def render(name, seconds):
    n = RATE * seconds
    values = [0.0] * n
    if name == 'HopefulMenu.wav':
        # Four original Am(add9), Fmaj7, Cadd9, Gsus2 arpeggios, soft felt/bell timbre.
        chords = [(220, 261.626, 329.628, 493.883), (174.614, 220, 261.626, 329.628),
                  (196, 261.626, 293.665, 391.995), (195.998, 246.942, 293.665, 440)]
        for bar, chord in enumerate(chords):
            for note in range(6):
                onset = int((bar * 3 + note * .5) * RATE)
                freq = chord[note % 4]
                for j in range(int(2.5 * RATE)):
                    t = j / RATE
                    attack = 1 - math.exp(-t / .025)
                    envelope = attack * math.exp(-t / .65) * (1 - t / 2.5) ** 2
                    tone = math.sin(math.tau * freq * t) + .2 * math.sin(math.tau * freq * 2 * t)
                    # Circular accumulation lets the final notes decay naturally across the seam.
                    values[(onset+j) % n] += .12 * envelope * tone
    else:
        rng = random.Random(621 if name == 'VillageWind.wav' else 623)
        # Integer cycle frequencies and phases: exact periodic textures, no white-noise seam.
        partials = []
        for i in range(70):
            harmonic = rng.randrange(180, 5200) if name == 'VillageWind.wav' else rng.randrange(120, 900)
            freq = harmonic / seconds
            amplitude = 1 / (1 + (freq / 220) ** 1.7)
            partials.append((harmonic, rng.random() * math.tau, amplitude))
        scale = .09 / math.sqrt(sum(a*a for _, _, a in partials))
        for i in range(n):
            phase = math.tau * i / n
            texture = sum(a * math.sin(k*phase+p) for k, p, a in partials)
            slow = .7 + .2 * math.sin(phase*2) + .1 * math.cos(phase*3)
            values[i] = scale * texture * slow
        if name == 'CrisisUndertone.wav':
            for i in range(n):
                phase = math.tau * i / n
                values[i] = .55*values[i] + .025*math.sin(phase*640)*(.65+.35*math.cos(phase*3))
    # Short circular seam blend. Only 15ms at each edge; sample zero at both endpoints.
    # Smooth edge slopes prevent a DC jump even for the arpeggio frequencies.
    edge = int(.015 * RATE)
    for i in range(edge):
        gain = math.sin(math.pi * .5 * i / edge) ** 2
        values[i] *= gain
        values[-1-i] *= gain
    peak = max(abs(v) for v in values)
    if peak > .35:
        values = [v * .35 / peak for v in values]
    return b''.join(struct.pack('<h', round(v * 32767)) for v in values)


def verify(path, expected_duration):
    with wave.open(str(path), 'rb') as wav:
        assert (wav.getnchannels(), wav.getsampwidth(), wav.getframerate(), wav.getcomptype()) == (1, 2, RATE, 'NONE')
        assert wav.getnframes() == expected_duration * RATE
        data = wav.readframes(wav.getnframes())
    samples = struct.unpack('<' + 'h' * (len(data)//2), data)
    peak = max(abs(x) for x in samples)/32768
    rms = math.sqrt(sum(x*x for x in samples)/len(samples))/32768
    jump = abs(samples[0]-samples[-1])/32768
    assert samples[0] == samples[-1] == 0 and jump == 0
    assert 0.001 < rms < .15 and peak <= .351
    assert abs(sum(samples)/len(samples)/32768) < .001
    # The seam slope is also bounded; this is numerical QA, not listening or an acoustic guarantee.
    assert max(abs(samples[1]-samples[0]),abs(samples[-1]-samples[-2])) <= 8
    return {'file': path.name, 'duration_seconds': expected_duration, 'bytes': path.stat().st_size,
            'peak': round(peak,6), 'rms': round(rms,6), 'seam_jump': jump,
            'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}


def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--check',action='store_true')
    args=parser.parse_args()
    for name,seconds in SPECS.items():
        path=ROOT/name
        if not args.check:
            ROOT.mkdir(parents=True,exist_ok=True)
            with wave.open(str(path),'wb') as wav:
                wav.setnchannels(1);wav.setsampwidth(2);wav.setframerate(RATE);wav.writeframes(render(name,seconds))
        print(json.dumps(verify(path,seconds)))


if __name__ == '__main__':
    main()
