class Tinycity < Formula
  desc "Ask any large language model from your terminal via OpenAI-compatible APIs"
  homepage "https://github.com/yetanotherchris/tinycity"
  version "2.1.0"
  license "MIT"

  on_macos do
    if Hardware::CPU.arm?
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.1.0/tinycity-v2.1.0-osx-arm64"
      sha256 "82abccb074bf4c44b89eb5d66f7ac5a2a3c8306bd4010009e829ae23f5f20a87"
    else
      url "https://github.com/yetanotherchris/tinycity/releases/download/v2.1.0/tinycity-v2.1.0-osx-x64"
      sha256 "fed6925dc89aa555b335dbec6004abbf8fc6593ce07fc9f8435d43ed1a39d627"
    end
  end

  on_linux do
    url "https://github.com/yetanotherchris/tinycity/releases/download/v2.1.0/tinycity-v2.1.0-linux-x64"
    sha256 "94adf15ca65d34b0f6ba5587e9a217a4bed747330bc18d76bf3cc7107847ded1"
  end

  def install
    if OS.mac?
      if Hardware::CPU.arm?
        bin.install "tinycity-v2.1.0-osx-arm64" => "tinycity"
      else
        bin.install "tinycity-v2.1.0-osx-x64" => "tinycity"
      end
    else
      bin.install "tinycity-v2.1.0-linux-x64" => "tinycity"
    end
  end

  test do
    assert_match "USAGE:", shell_output("#{bin}/tinycity --help")
  end
end








