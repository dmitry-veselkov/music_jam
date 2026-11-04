import { useCallback, useRef, useState } from "react";

let activeAudio: HTMLAudioElement | null = null;

export function useAudioPlayer() {
  const audioRef = useRef<HTMLAudioElement | null>(null);
  const [isPlaying, setIsPlaying] = useState(false);
  const [currentTime, setCurrentTime] = useState(0);
  const [duration, setDuration] = useState(0);

  const toggle = useCallback(async () => {
    const audio = audioRef.current;
    if (!audio) return;
    if (audio.paused) {
      await audio.play();
    } else {
      audio.pause();
    }
  }, []);

  const seek = useCallback((time: number) => {
    if (audioRef.current) audioRef.current.currentTime = time;
    setCurrentTime(time);
  }, []);

  const reset = useCallback(() => {
    audioRef.current?.pause();
    seek(0);
  }, [seek]);

  const audioProps = {
    ref: audioRef,
    onPlay: (e: React.SyntheticEvent<HTMLAudioElement>) => {
      const audio = e.currentTarget;
      if (activeAudio && activeAudio !== audio) activeAudio.pause();
      activeAudio = audio;
      setIsPlaying(true);
    },
    onPause: (e: React.SyntheticEvent<HTMLAudioElement>) => {
      if (activeAudio === e.currentTarget) activeAudio = null;
      setIsPlaying(false);
    },
    onEnded: () => setIsPlaying(false),
    onTimeUpdate: () => setCurrentTime(audioRef.current?.currentTime ?? 0),
    onLoadedMetadata: () => setDuration(audioRef.current?.duration ?? 0),
  };

  return { audioProps, isPlaying, currentTime, duration, toggle, seek, reset };
}