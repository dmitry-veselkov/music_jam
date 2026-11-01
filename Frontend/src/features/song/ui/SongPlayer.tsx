import { useAudioPlayer } from "@/shared/lib/audio";
import { AudioToolbar } from "@/shared/ui/AudioToolBar/AudioToolBar.tsx";

type Props = {
  title: string;
  songUrl: string;
};

export function SongPlayer({ title, songUrl }: Props) {
  const { audioProps, isPlaying, currentTime, duration, toggle, seek } = useAudioPlayer();

  return (
    <>
      <audio {...audioProps} src={songUrl} preload="metadata" />
      <AudioToolbar
        title={title}
        isPlaying={isPlaying}
        currentTime={currentTime}
        duration={duration}
        disabled={!duration}
        onToggle={toggle}
        onSeek={seek}
      />
    </>
  );
}