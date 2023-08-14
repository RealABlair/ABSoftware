nf={}

function tl(T) c=0 for _ in pairs(T) do if _>=0 then c=c+1 end end return c end

function fltr(nid,n,c)
    anv=0
    if nf[nid]==nil then nf[nid]={} nf[nid][0]=n nf[nid][-1]=1%c
    else nf[nid][nf[nid][-1]]=n nf[nid][-1]=(nf[nid][-1]+1)%c
    end
    for i,v in pairs(nf[nid]) do if i>=0 then anv=anv+v end end
    return anv/tl(nf[nid])
end
