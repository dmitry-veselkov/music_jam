export type TeamState = 'ready' | 'disconnected';

export type Team = {
  teamId : string;
  teamName : string;
  state: TeamState;
}